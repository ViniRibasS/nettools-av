using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using NetToolsAV.Core;
using NetToolsAV.Infrastructure;

namespace NetToolsAV.Desktop;

public partial class MainWindow : Window
{
    private readonly string dataDirectory;
    private SessionStore? store;
    private AnalysisSession? session;
    private IReadOnlyList<EthernetSnapshot> interfaces = [];
    private bool rendering;
    private bool busy;
    private bool dark;
    private CaptureSnapshot? capture;
    private CaptureClient? captureClient;
    private CancellationTokenSource? importStop;
    private bool closingReady;

    public MainWindow(string directory)
    {
        dataDirectory = directory;
        InitializeComponent();
    }

    private async void WindowLoaded(object sender, RoutedEventArgs e)
    {
        await Run(async () =>
        {
            store = await Task.Run(() => new SessionStore(dataDirectory));
            var sessions = await Task.Run(store.Load);
            if (sessions.Count == 0)
            {
                var initial = new AnalysisSession("Análise de protocolos", "Ponto de medição local");
                foreach (var id in new[] { "mdns", "igmp3", "ptp2", "rtp", "qdp", "icmp" }) initial.AddProtocol(id);
                await Task.Run(() => store.Save(initial));
            }
            interfaces = await Task.Run(EthernetInventory.Read);
            await Reload();
        });
    }

    private async Task Run(Func<Task> operation)
    {
        if (busy) return;
        busy = true;
        Root.IsEnabled = false;
        StatusText.Text = "Processando dados locais…";
        try
        {
            await operation();
            StatusText.Text = capture is null ? $"Sessão local · {session?.ProtocolIds.Count ?? 0} protocolos / perfis · nenhuma análise executada" : $"{capture.Status} · {capture.Packets:N0} pacotes · inspeção parcial";
        }
        catch (Exception exception)
        {
            StatusText.Text = "A operação não foi concluída. Consulte a mensagem de erro.";
            MessageBox.Show(this, exception.Message + "\n\nOs dados não foram marcados como validados.", "NetTools AV — operação não concluída", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally
        {
            Root.IsEnabled = true;
            busy = false;
        }
    }

    private async Task Reload(Guid? preferred = null)
    {
        if (store is null) return;
        var selected = preferred ?? session?.Id;
        var loaded = await Task.Run(store.Load);
        rendering = true;
        try
        {
            SessionList.ItemsSource = loaded;
            session = loaded.FirstOrDefault(s => s.Id == selected) ?? loaded.FirstOrDefault();
            SessionList.SelectedItem = session;
            if (session is not null && captureClient?.IsRunning != true) capture = CaptureArchive.Latest(dataDirectory, session.Id);
        }
        finally { rendering = false; }
        RenderSession();
    }

    private AnalysisSession CopySession()
    {
        if (session is null) throw new InvalidOperationException("Selecione uma sessão.");
        var copy = new AnalysisSession(session.Name, session.Location, session.Id, session.CreatedUtc, session.CatalogVersion);
        copy.SelectInterface(session.InterfaceId);
        foreach (var id in session.ProtocolIds) copy.AddProtocol(id);
        return copy;
    }

    private void RenderSession()
    {
        if (session is null) return;
        rendering = true;
        try
        {
            SessionTitle.Text = Breadcrumb.Text = session.Name;
            SessionSubtitle.Text = $"{session.Location} · criada em {session.CreatedUtc.ToLocalTime():dd/MM/yyyy HH:mm}";
            ProtocolCount.Text = $"Protocolos / perfis · {session.ProtocolIds.Count}";
            var selectedId = (ProtocolGrid.SelectedItem as ProtocolView)?.Id;
            var query = SearchBox.Text.Trim();
            var ids = session.ProtocolIds.Concat(capture?.Protocols.Select(p => p.Id) ?? []).Distinct();
            var protocols = ids.Select(ProtocolCatalog.Get).Where(p =>
                $"{p.Name} {p.Family} {p.Version}".Contains(query, StringComparison.CurrentCultureIgnoreCase)).Select(p => new ProtocolView(p, capture)).ToArray();
            ProtocolCount.Text = $"Protocolos / perfis · {protocols.Length}";
            ProtocolGrid.ItemsSource = protocols;
            ProtocolGrid.SelectedItem = protocols.FirstOrDefault(p => p.Id == selectedId) ?? protocols.FirstOrDefault();
            CatalogPicker.ItemsSource = ProtocolCatalog.All.Where(p => !session.ProtocolIds.Contains(p.Id)).ToArray();
            CatalogPicker.SelectedIndex = -1;
            InterfacePicker.ItemsSource = interfaces;
            InterfacePicker.SelectedItem = interfaces.FirstOrDefault(i => i.Id == session.InterfaceId);
            SessionNote.Text = query.Length > 0 && protocols.Length == 0
                ? "Nenhum protocolo da sessão corresponde à busca."
                : capture is null ? "Selecione a interface e inicie uma captura, ou importe um PCAPNG."
                : $"{capture.Status} · {capture.Packets:N0} pacotes · {capture.Bytes:N0} bytes · descartes Npcap: {capture.DriverDropped?.ToString() ?? "indisponível"}";
            InterfaceDetails.Text = (InterfacePicker.SelectedItem as EthernetSnapshot)?.Summary
                ?? (session.InterfaceId is null
                    ? interfaces.Count == 0 ? "Nenhuma interface Ethernet foi informada pelo Windows." : "Selecione explicitamente a interface desta sessão."
                    : "A interface gravada não está disponível. Selecione outra explicitamente; não houve troca automática.");
        }
        finally { rendering = false; }
        RenderProtocol();
    }

    private void RenderProtocol()
    {
        var protocol = ProtocolGrid.SelectedItem as ProtocolView;
        ProtocolTitle.Text = protocol?.Name ?? "Selecione um protocolo";
        ProtocolFamily.Text = protocol is null ? "" : $"{protocol.Family} · {protocol.Version}";
        ProtocolTransport.Text = protocol?.Transport ?? "";
        ProtocolLimit.Text = protocol?.Limitation ?? "A sessão organiza análises por protocolo. Nenhuma observação foi coletada.";
        AnalysisState.Text = protocol?.State ?? "Não executado";
        AnalysisDetail.Text = capture is null ? "Nenhuma observação coletada." : $"{protocol?.Presence} · {protocol?.Packets} pacotes\n{capture.StartedUtc.ToLocalTime():HH:mm:ss} – {capture.UpdatedUtc.ToLocalTime():HH:mm:ss}\nEstruturas inválidas/incompletas: {capture.Malformed}; truncados: {capture.Truncated}.\n{capture.Error}";
    }

    private void SessionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (rendering) return;
        session = SessionList.SelectedItem as AnalysisSession;
        try { capture = session is null ? null : CaptureArchive.Latest(dataDirectory, session.Id); }
        catch (Exception exception) when (exception is IOException or System.Text.Json.JsonException) { capture = null; MessageBox.Show(this, exception.Message, "Resumo não carregado"); }
        RenderSession();
    }

    private void ProtocolChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!rendering) RenderProtocol();
    }

    private void SearchChanged(object sender, TextChangedEventArgs e)
    {
        if (IsLoaded && !rendering) RenderSession();
    }

    private async void AddProtocolClick(object sender, RoutedEventArgs e)
    {
        if (store is null || CatalogPicker.SelectedItem is not ProtocolDefinition protocol) return;
        await Run(async () =>
        {
            var copy = CopySession();
            copy.AddProtocol(protocol.Id);
            await Task.Run(() => store.Save(copy));
            SearchBox.Clear();
            await Reload(copy.Id);
            ProtocolGrid.SelectedItem = ProtocolGrid.Items.Cast<ProtocolView>().First(p => p.Id == protocol.Id);
        });
    }

    private async void InterfaceChanged(object sender, SelectionChangedEventArgs e)
    {
        if (rendering || store is null || session is null || InterfacePicker.SelectedItem is not EthernetSnapshot selected) return;
        await Run(async () =>
        {
            var copy = CopySession();
            copy.SelectInterface(selected.Id);
            await Task.Run(() => store.Save(copy));
            await Reload(copy.Id);
        });
        RenderSession(); // Restore persisted selection after a failed save.
    }

    private async void RefreshClick(object sender, RoutedEventArgs e) => await Run(async () =>
    {
        interfaces = await Task.Run(EthernetInventory.Read);
        RenderSession();
    });

    private async void NewSessionClick(object sender, RoutedEventArgs e)
    {
        if (store is null || captureClient?.IsRunning == true) return;
        var dialog = new SessionDialog { Owner = this };
        if (dialog.ShowDialog() != true || dialog.Result is null) return;
        await Run(async () =>
        {
            await Task.Run(() => store.Save(dialog.Result));
            SearchBox.Clear();
            await Reload(dialog.Result.Id);
        });
    }

    private void SearchClick(object sender, RoutedEventArgs e) => SearchBox.Focus();
    private void ProtocolsClick(object sender, RoutedEventArgs e) => ProtocolGrid.Focus();
    private void CapturesClick(object sender, RoutedEventArgs e)
    {
        if (session is null) return;
        try
        {
            var window = new CaptureHistoryWindow(dataDirectory, session.Id, capture) { Owner = this };
            window.ShowDialog();
            if (window.SelectedCapture is { } selected && captureClient?.IsRunning != true) { capture = selected; RenderSession(); }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or System.Text.Json.JsonException) { MessageBox.Show(this, exception.Message, "Histórico indisponível"); }
    }

    private async void StartCaptureClick(object sender, RoutedEventArgs e)
    {
        if (session is null || captureClient?.IsRunning == true) return;
        if (InterfacePicker.SelectedItem is not EthernetSnapshot nic) { MessageBox.Show(this, "Selecione explicitamente uma interface Ethernet para capturar."); return; }
        var dialog = new CaptureDialog(session, CaptureArchive.NewDirectory(dataDirectory, session.Id), nic.DisplayName) { Owner = this };
        if (dialog.ShowDialog() != true || dialog.Result is null) return;
        try
        {
            captureClient?.Dispose(); captureClient = new();
            captureClient.Updated += snapshot => Dispatcher.BeginInvoke(() =>
            {
                capture = snapshot; RenderSession();
                var active = snapshot.Status == "Capturando";
                SetCapturing(active); StatusText.Text = snapshot.Error ?? $"{snapshot.Status} · {snapshot.Packets:N0} pacotes · inspeção parcial";
            });
            capture = new() { SessionId = session.Id, Status = "Preparando", Source = nic.Name };
            SetCapturing(true); StatusText.Text = "Abrindo Npcap na interface selecionada…";
            await captureClient.Start(dialog.Result);
        }
        catch (Exception exception) { SetCapturing(false); MessageBox.Show(this, exception.Message, "Captura não iniciada"); }
    }
    private void SetCapturing(bool active)
    {
        SessionList.IsEnabled = InterfacePicker.IsEnabled = StartCaptureButton.IsEnabled = ImportCaptureButton.IsEnabled = !active;
        StopCaptureButton.IsEnabled = active;
    }
    private async void StopCaptureClick(object sender, RoutedEventArgs e)
    {
        StopCaptureButton.IsEnabled = false;
        try { if (captureClient is not null) await captureClient.Stop(); }
        catch (Exception exception) { MessageBox.Show(this, exception.Message, "Erro ao encerrar captura"); }
        finally { SetCapturing(false); }
    }
    private async void ImportCaptureClick(object sender, RoutedEventArgs e)
    {
        if (session is null || captureClient?.IsRunning == true) return;
        var dialog = new OpenFileDialog { Title = "Importar captura PCAPNG", Filter = "PCAPNG (*.pcapng)|*.pcapng", CheckFileExists = true };
        if (dialog.ShowDialog(this) != true) return;
        importStop = new();
        await Run(async () =>
        {
            capture = await Task.Run(() => CaptureArchive.Import(dialog.FileName, CaptureArchive.NewDirectory(dataDirectory, session.Id), session.Id, importStop.Token));
            RenderSession();
        });
        importStop.Dispose(); importStop = null;
    }
    private async void WindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (closingReady) return;
        if (importStop is not null) { e.Cancel = true; importStop.Cancel(); return; }
        if (captureClient?.IsRunning != true) { captureClient?.Dispose(); return; }
        e.Cancel = true;
        try { await captureClient.Stop(); }
        catch (Exception exception) { MessageBox.Show(this, exception.Message, "Encerramento da captura não confirmado"); }
        finally { captureClient.Dispose(); closingReady = true; Close(); }
    }

    private async void ReportClick(object sender, RoutedEventArgs e)
    {
        if (session is null) return;
        var dialog = new SaveFileDialog { Title = "Exportar relatório por protocolos", Filter = "Relatório HTML (*.html)|*.html|Resumo textual (*.txt)|*.txt|Dados JSON (*.json)|*.json", FileName = $"nettools-av-{session.Id:N}" };
        if (dialog.ShowDialog(this) == true)
        {
            var text = CaptureArchive.Report(session, capture, Path.GetExtension(dialog.FileName).TrimStart('.').ToLowerInvariant());
            await Run(async () => await File.WriteAllTextAsync(dialog.FileName, text, System.Text.Encoding.UTF8));
        }
    }

    private void ThemeClick(object sender, RoutedEventArgs e) { dark = !dark; App.SetTheme(dark); }
    private void SidebarClick(object sender, RoutedEventArgs e)
    {
        var visible = Sidebar.Visibility == Visibility.Visible;
        Sidebar.Visibility = visible ? Visibility.Collapsed : Visibility.Visible;
        SidebarColumn.Width = new GridLength(visible ? 0 : 232);
    }
    private void InspectorClick(object sender, RoutedEventArgs e)
    {
        var visible = Inspector.Visibility == Visibility.Visible;
        Inspector.Visibility = visible ? Visibility.Collapsed : Visibility.Visible;
        InspectorColumn.Width = new GridLength(visible ? 0 : 288);
    }
}

