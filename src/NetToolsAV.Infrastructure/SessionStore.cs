using System.Globalization;
using Microsoft.Data.Sqlite;
using NetToolsAV.Core;

namespace NetToolsAV.Infrastructure;

public sealed class SessionStore
{
    private readonly string connectionString;
    private readonly object gate = new();
    public string DatabasePath { get; }

    public SessionStore(string directory)
    {
        Directory.CreateDirectory(directory);
        DatabasePath = Path.Combine(Path.GetFullPath(directory), "nettools-av.sqlite");
        connectionString = new SqliteConnectionStringBuilder { DataSource = DatabasePath, ForeignKeys = true, Pooling = false }.ToString();
        using var db = Open();
        using var version = db.CreateCommand();
        version.CommandText = "PRAGMA user_version";
        var schema = Convert.ToInt32(version.ExecuteScalar(), CultureInfo.InvariantCulture);
        if (schema > 1) throw new InvalidDataException("Banco criado por uma versão mais nova. Use uma versão compatível; o arquivo foi preservado.");
        using var transaction = db.BeginTransaction();
        using var command = db.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS sessions (
                id TEXT PRIMARY KEY, name TEXT NOT NULL, location TEXT NOT NULL,
                created_utc TEXT NOT NULL, interface_id TEXT, catalog_version TEXT NOT NULL);
            CREATE TABLE IF NOT EXISTS session_protocols (
                session_id TEXT NOT NULL REFERENCES sessions(id) ON DELETE CASCADE,
                protocol_id TEXT NOT NULL, ordinal INTEGER NOT NULL,
                PRIMARY KEY(session_id, protocol_id));
            PRAGMA user_version = 1;
            """;
        command.ExecuteNonQuery();
        transaction.Commit();
    }

    private SqliteConnection Open()
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }

    public IReadOnlyList<AnalysisSession> Load()
    {
        lock (gate)
        {
            using var db = Open();
            using var transaction = db.BeginTransaction(deferred: true);
            var result = new List<AnalysisSession>();
            using var command = db.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "SELECT id,name,location,created_utc,interface_id,catalog_version FROM sessions ORDER BY created_utc";
            using (var reader = command.ExecuteReader())
                while (reader.Read())
                {
                    var session = new AnalysisSession(reader.GetString(1), reader.GetString(2), Guid.Parse(reader.GetString(0)), DateTimeOffset.Parse(reader.GetString(3), CultureInfo.InvariantCulture), reader.GetString(5));
                    session.SelectInterface(reader.IsDBNull(4) ? null : reader.GetString(4));
                    result.Add(session);
                }
            foreach (var session in result)
            {
                using var protocols = db.CreateCommand();
                protocols.Transaction = transaction;
                protocols.CommandText = "SELECT protocol_id FROM session_protocols WHERE session_id=$id ORDER BY ordinal";
                protocols.Parameters.AddWithValue("$id", session.Id.ToString());
                using var reader = protocols.ExecuteReader();
                while (reader.Read()) session.AddProtocol(reader.GetString(0));
            }
            transaction.Commit();
            return result;
        }
    }

    public void Save(AnalysisSession session)
    {
        lock (gate)
        {
            using var db = Open();
            using var transaction = db.BeginTransaction();
            using var command = db.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = """
                INSERT INTO sessions(id,name,location,created_utc,interface_id,catalog_version)
                VALUES($id,$name,$location,$created,$interface,$catalog)
                ON CONFLICT(id) DO UPDATE SET interface_id=excluded.interface_id;
                DELETE FROM session_protocols WHERE session_id=$id;
                """;
            command.Parameters.AddWithValue("$id", session.Id.ToString());
            command.Parameters.AddWithValue("$name", session.Name);
            command.Parameters.AddWithValue("$location", session.Location);
            command.Parameters.AddWithValue("$created", session.CreatedUtc.ToString("O", CultureInfo.InvariantCulture));
            command.Parameters.AddWithValue("$interface", (object?)session.InterfaceId ?? DBNull.Value);
            command.Parameters.AddWithValue("$catalog", session.CatalogVersion);
            command.ExecuteNonQuery();
            for (var index = 0; index < session.ProtocolIds.Count; index++)
            {
                using var insert = db.CreateCommand();
                insert.Transaction = transaction;
                insert.CommandText = "INSERT INTO session_protocols(session_id,protocol_id,ordinal) VALUES($id,$protocol,$ordinal)";
                insert.Parameters.AddWithValue("$id", session.Id.ToString());
                insert.Parameters.AddWithValue("$protocol", session.ProtocolIds[index]);
                insert.Parameters.AddWithValue("$ordinal", index);
                insert.ExecuteNonQuery();
            }
            transaction.Commit();
        }
    }
}
