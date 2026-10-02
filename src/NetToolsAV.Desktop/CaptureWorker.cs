using System.Diagnostics;
using System.IO;
using System.Text.Json;
using NetToolsAV.Core;
using NetToolsAV.Infrastructure;

namespace NetToolsAV.Desktop;

public static class CaptureWorker
{
    public static int Run()
    {
        CaptureRequest? request = null;
        CaptureSnapshot? lastSnapshot = null;
        try
        {
            var line = Console.ReadLine();
            if (line is null || line.Length > 16384) throw new InvalidDataException("Requisição ausente ou muito grande.");
            request = JsonSerializer.Deserialize<CaptureRequest>(line) ?? throw new InvalidDataException("Requisição inválida.");
            using var stop = new CancellationTokenSource();
            _ = Task.Run(() => { Console.ReadLine(); try { stop.Cancel(); } catch (ObjectDisposedException) { } }); // STOP or parent EOF.
            var result = NpcapCapture.Run(request, snapshot => { lastSnapshot = snapshot; Send(request.Directory, snapshot); }, stop.Token);
            lastSnapshot = result;
            CaptureArchive.Seal(request.Directory); Send(request.Directory, result);
            return result.Status == "Falha" ? 1 : 0;
        }
        catch (Exception exception)
        {
            var result = (lastSnapshot ?? new CaptureSnapshot { SessionId = request?.SessionId ?? Guid.Empty }) with { Status = "Falha", Error = exception is DllNotFoundException ? "Npcap não encontrado. Instale o driver separadamente conforme licença e permissões locais." : exception.Message };
            try { if (request is not null) CaptureArchive.Save(request.Directory, result); Console.WriteLine(JsonSerializer.Serialize(result)); }
            catch (IOException) { }
            return 1;
        }
    }
    private static void Send(string directory, CaptureSnapshot snapshot)
    {
        CaptureArchive.Save(directory, snapshot);
        Console.WriteLine(JsonSerializer.Serialize(snapshot)); Console.Out.Flush();
    }
}

public sealed class CaptureClient : IDisposable
{
    private Process? worker;
    private Task? reader;
    public bool IsRunning => worker is { HasExited: false };
    public event Action<CaptureSnapshot>? Updated;
    public async Task Start(CaptureRequest request)
    {
        if (IsRunning) throw new InvalidOperationException("Captura já ativa.");
        var info = new ProcessStartInfo(Environment.ProcessPath!) { UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Hidden };
        info.ArgumentList.Add("--capture-worker");
        worker = Process.Start(info) ?? throw new IOException("Host de captura não iniciou.");
        var process = worker;
        reader = Task.Run(async () =>
        {
            CaptureSnapshot? last = null;
            var errors = process.StandardError.ReadToEndAsync();
            try
            {
                while (await process.StandardOutput.ReadLineAsync() is { } line)
                {
                    if (line.Length > 1024 * 1024) throw new InvalidDataException("Resposta do host acima do limite.");
                    last = JsonSerializer.Deserialize<CaptureSnapshot>(line) ?? throw new InvalidDataException("Resposta vazia."); Updated?.Invoke(last);
                }
                await process.WaitForExitAsync(); var error = await errors;
                if (last is null || last.Status == "Capturando")
                {
                    var failure = (last ?? new CaptureSnapshot { SessionId = request.SessionId }) with { Status = "Falha", Error = "Host encerrou sem confirmação final. " + error[..Math.Min(error.Length, 500)] };
                    CaptureArchive.Save(request.Directory, failure); Updated?.Invoke(failure);
                }
            }
            catch (Exception ex) when (ex is IOException or JsonException or InvalidOperationException)
            {
                Updated?.Invoke(new CaptureSnapshot { SessionId = request.SessionId, Status = "Falha", Error = ex.Message });
                if (!process.HasExited) process.Kill(true);
            }
        });
        await process.StandardInput.WriteLineAsync(JsonSerializer.Serialize(request)); await process.StandardInput.FlushAsync();
    }
    public async Task Stop()
    {
        if (worker is null) return;
        if (!worker.HasExited)
        {
            worker.StandardInput.Close();
            var exited = worker.WaitForExitAsync();
            if (await Task.WhenAny(exited, Task.Delay(5000)) != exited) { worker.Kill(true); await exited; }
        }
        if (reader is not null) await reader;
    }
    public void Dispose() { if (IsRunning) worker!.StandardInput.Close(); worker?.Dispose(); }
}
