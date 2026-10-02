using System.Windows;
using System.Windows.Controls;
using NetToolsAV.Core;

namespace NetToolsAV.Desktop;

public sealed class SessionDialog : Window
{
    public AnalysisSession? Result { get; private set; }
    public SessionDialog()
    {
        SetResourceReference(StyleProperty, typeof(Window));
        Title = "Nova sessão de protocolos";
        Width = 460;
        SizeToContent = SizeToContent.Height;
        ResizeMode = ResizeMode.NoResize;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        var panel = new StackPanel { Margin = new Thickness(25) };
        panel.Children.Add(new TextBlock { Text = "Nova sessão", FontSize = 23, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 0, 0, 20) });
        var name = new TextBox { MaxLength = 70, Margin = new Thickness(0, 5, 0, 16) };
        var location = new TextBox { MaxLength = 100, Margin = new Thickness(0, 5, 0, 20) };
        System.Windows.Automation.AutomationProperties.SetName(name, "Nome da sessão");
        System.Windows.Automation.AutomationProperties.SetName(location, "Ponto de medição");
        panel.Children.Add(new Label { Content = "Nome da sessão", Target = name });
        panel.Children.Add(name);
        panel.Children.Add(new Label { Content = "Ponto de medição", Target = location });
        panel.Children.Add(location);
        var actions = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        var cancel = new Button { Content = "Cancelar", IsCancel = true, Margin = new Thickness(0, 0, 8, 0) };
        var create = new Button { Content = "Criar sessão", IsDefault = true };
        create.Click += (_, _) =>
        {
            try { Result = new AnalysisSession(name.Text, location.Text); DialogResult = true; }
            catch (ArgumentException exception) { MessageBox.Show(this, exception.Message, "Verifique os campos", MessageBoxButton.OK, MessageBoxImage.Information); }
        };
        actions.Children.Add(cancel);
        actions.Children.Add(create);
        panel.Children.Add(actions);
        Content = panel;
        Loaded += (_, _) => name.Focus();
    }
}
