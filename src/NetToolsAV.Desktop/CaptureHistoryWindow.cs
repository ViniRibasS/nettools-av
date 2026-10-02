using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using NetToolsAV.Core;
using NetToolsAV.Infrastructure;
using Microsoft.Win32;

namespace NetToolsAV.Desktop;

public sealed class CaptureHistoryWindow : Window
{
    public CaptureSnapshot? SelectedCapture { get; private set; }
    private sealed record Entry(string Path, CaptureSnapshot Snapshot)
    {
        public string Label => $"{Snapshot.StartedUtc.ToLocalTime():dd/MM HH:mm:ss} · {Snapshot.Status} · {Snapshot.Packets:N0} pacotes";
    }
    public CaptureHistoryWindow(string root, Guid session, CaptureSnapshot? current)
    {
        SetResourceReference(StyleProperty, typeof(Window));
        Title = "Capturas e evidências por protocolo"; Width = 1100; Height = 720; MinWidth = 900; MinHeight = 550; WindowStartupLocation = WindowStartupLocation.CenterOwner;
        var grid = new Grid { Margin = new Thickness(24) };
        grid.RowDefinitions.Add(new() { Height = GridLength.Auto }); grid.RowDefinitions.Add(new() { Height = GridLength.Auto }); grid.RowDefinitions.Add(new() { Height = new GridLength(1, GridUnitType.Star) }); grid.RowDefinitions.Add(new() { Height = GridLength.Auto });
        var entries = new List<Entry>(); var errors = 0; var directory = Path.Combine(root, "captures", session.ToString("N"));
        if (Directory.Exists(directory))
        {
            foreach (var file in Directory.EnumerateFiles(directory, "summary.json", SearchOption.AllDirectories).OrderByDescending(File.GetLastWriteTimeUtc).Take(200))
            {
                try
                {
                    if (new FileInfo(file).Length > 1024 * 1024) throw new InvalidDataException();
                    var snapshot = JsonSerializer.Deserialize<CaptureSnapshot>(File.ReadAllText(file));
                    if (snapshot is null) throw new InvalidDataException();
                    CaptureArchive.Validate(snapshot, session);
                    if (snapshot.Status == "Capturando" && snapshot.Id != current?.Id) snapshot = snapshot with { Status = "Interrompida", Error = "Encerramento não confirmado." };
                    entries.Add(new(Path.GetDirectoryName(file)!, snapshot));
                }
                catch (Exception exception) when (exception is IOException or JsonException) { errors++; }
            }
        }
        var picker = new ComboBox { ItemsSource = entries, DisplayMemberPath = "Label", Tag = "Selecione uma captura", Margin = new Thickness(0, 0, 0, 14) };
        System.Windows.Automation.AutomationProperties.SetName(picker, "Histórico de capturas");
        var details = new TextBlock { TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 18) }; Grid.SetRow(details, 1);
        var table = new DataGrid { AutoGenerateColumns = false, IsReadOnly = true, RowHeight = 45 };
        table.Columns.Add(new DataGridTextColumn { Header = "Pacote", Binding = new System.Windows.Data.Binding("Number"), Width = 65 });
        table.Columns.Add(new DataGridTextColumn { Header = "Protocolo", Binding = new System.Windows.Data.Binding("Protocol"), Width = 85 });
        table.Columns.Add(new DataGridTextColumn { Header = "Origem", Binding = new System.Windows.Data.Binding("Source"), Width = 160 });
        table.Columns.Add(new DataGridTextColumn { Header = "Destino", Binding = new System.Windows.Data.Binding("Destination"), Width = 160 });
        table.Columns.Add(new DataGridTextColumn { Header = "Evidência de cabeçalho", Binding = new System.Windows.Data.Binding("Detail"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
        Grid.SetRow(table, 2);
        var actions = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 14, 0, 0) }; Grid.SetRow(actions, 3);
        var export = new Button { Content = "Exportar evidências ZIP", Margin = new Thickness(0, 0, 10, 0), IsEnabled = false };
        var use = new Button { Content = "Exibir esta captura na sessão", IsEnabled = false };
        actions.Children.Add(export); actions.Children.Add(use);
        export.Click += async (_, _) =>
        {
            if (picker.SelectedItem is not Entry entry) return;
            var dialog = new SaveFileDialog { Title = "Exportar evidências sem modificar os originais", Filter = "Evidências (*.zip)|*.zip", FileName = $"nettools-av-{entry.Snapshot.Id:N}.zip" };
            if (dialog.ShowDialog(this) != true) return;
            if (File.Exists(dialog.FileName)) { MessageBox.Show(this, "Escolha um nome novo para preservar o arquivo existente."); return; }
            export.IsEnabled = false;
            try { await Task.Run(() => CaptureArchive.Export(entry.Path, dialog.FileName)); }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException) { MessageBox.Show(this, exception.Message, "Exportação não concluída"); }
            finally { export.IsEnabled = true; }
        };
        use.Click += (_, _) => { if (picker.SelectedItem is Entry entry) { SelectedCapture = entry.Snapshot; DialogResult = true; } };
        picker.SelectionChanged += (_, _) =>
        {
            if (picker.SelectedItem is not Entry entry) return;
            var s = entry.Snapshot;
            details.Text = $"{s.Source}\nFiltro: {(s.Filter.Length == 0 ? "todos os pacotes visíveis" : s.Filter)} · {s.Bytes:N0} bytes · truncados {s.Truncated:N0} · estruturas inválidas/incompletas {s.Malformed:N0}\n{s.Limitations}\n{s.Error}\nPasta: {entry.Path}";
            table.ItemsSource = s.Evidence; use.IsEnabled = true; export.IsEnabled = s.Status != "Capturando";
        };
        grid.Children.Add(picker); grid.Children.Add(details); grid.Children.Add(table); grid.Children.Add(actions); Content = grid;
        if (entries.Count > 0) picker.SelectedIndex = 0;
        else details.Text = "Nenhuma captura concluída/importada nesta sessão. Use Capturar ou Importar na janela principal.";
        if (errors > 0) details.Text += $"\n{errors} resumos inválidos foram ignorados; arquivos preservados.";
    }
}
