using System.IO;
using System.Windows;
using System.Windows.Media;

namespace NetToolsAV.Desktop;

public partial class App : Application
{
    // Both UI and worker retain a handle so Setup cannot replace files during capture.
    private readonly Mutex installationGuard = new(false, @"Local\NetToolsAV.Application");

    protected override void OnExit(ExitEventArgs e)
    {
        installationGuard.Dispose();
        base.OnExit(e);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        if (e.Args.Contains("--capture-worker")) { Shutdown(CaptureWorker.Run()); return; }
        base.OnStartup(e);
        var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NetToolsAV");
        for (var i = 0; i < e.Args.Length; i++)
        {
            if (e.Args[i] == "--data-dir" && i + 1 < e.Args.Length) directory = Path.GetFullPath(e.Args[++i]);
        }
        new MainWindow(directory).Show();
    }

    public static void SetTheme(bool dark)
    {
        var colors = dark
            ? new[] { "#20211F", "#181917", "#242522", "#ECECE7", "#A1A29A", "#363832", "#35372F", "#ECEEE5", "#24251F" }
            : new[] { "#FCFCFB", "#F2F1EF", "#F8F8F6", "#292A28", "#70726C", "#E5E5E0", "#E7E7E2", "#272925", "#FFFFFF" };
        var keys = new[] { "CanvasBrush", "SideBrush", "PanelBrush", "TextBrush", "MutedBrush", "LineBrush", "SelectedBrush", "StrongBrush", "OnStrongBrush" };
        for (var i = 0; i < keys.Length; i++) Current.Resources[keys[i]] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors[i]));
    }
}
