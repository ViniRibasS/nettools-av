using System.Windows;
using System.Windows.Controls;
using NetToolsAV.Core;
using NetToolsAV.Infrastructure;

namespace NetToolsAV.Desktop;

public sealed class CaptureDialog : Window
{
    public CaptureRequest? Result { get; private set; }
    public CaptureDialog(AnalysisSession session, string directory, string interfaceName)
    {
        SetResourceReference(StyleProperty, typeof(Window));
        Title = "Nova captura por protocolos"; Width = 530; SizeToContent = SizeToContent.Height; ResizeMode = ResizeMode.NoResize; WindowStartupLocation = WindowStartupLocation.CenterOwner;
        var panel = new StackPanel { Margin = new Thickness(26) };
        panel.Children.Add(new TextBlock { Text = "Capturar tráfego", FontSize = 24, FontWeight = FontWeights.SemiBold });
        panel.Children.Add(new TextBlock { Text = interfaceName, Margin = new Thickness(0, 10, 0, 18), TextWrapping = TextWrapping.Wrap });
        var filter = new TextBox { MaxLength = 1024, ToolTip = "Sintaxe BPF; vazio inclui todo o tráfego visível." };
        var duration = new TextBox { Text = "60", MaxLength = 4 };
        var limit = new TextBox { Text = "256", MaxLength = 4 };
        var promiscuous = new CheckBox { Content = "Modo promíscuo (ex.: porta espelhada)", Margin = new Thickness(0, 15, 0, 15) };
        promiscuous.SetResourceReference(ForegroundProperty, "TextBrush");
        Field("Filtro de captura BPF (opcional)", filter); Field("Duração máxima em segundos (1–3600)", duration); Field("Limite total em MiB (1–1024)", limit);
        panel.Children.Add(promiscuous);
        panel.Children.Add(new TextBlock { Text = "A captura pode conter conteúdo dos pacotes. Arquivos ficam locais, com segmentos de até 32 MiB. Parar encerra a coleta; nenhum pacote de teste é enviado.", TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 20) });
        var buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        buttons.Children.Add(new Button { Content = "Cancelar", IsCancel = true, Margin = new Thickness(0, 0, 10, 0) });
        var start = new Button { Content = "Iniciar captura", IsDefault = true };
        start.Click += (_, _) =>
        {
            if (!int.TryParse(duration.Text, out var seconds) || seconds is < 1 or > 3600 || !int.TryParse(limit.Text, out var mib) || mib is < 1 or > 1024)
            { MessageBox.Show(this, "Confira duração e limite dentro dos intervalos informados."); return; }
            Result = new(session.Id, session.InterfaceId!, directory, filter.Text.Trim(), promiscuous.IsChecked == true, seconds, mib); DialogResult = true;
        };
        buttons.Children.Add(start); panel.Children.Add(buttons); Content = panel;
        void Field(string label, TextBox input) { panel.Children.Add(new Label { Content = label, Target = input, Margin = new Thickness(0, 7, 0, 3) }); System.Windows.Automation.AutomationProperties.SetName(input, label); panel.Children.Add(input); }
    }
}
