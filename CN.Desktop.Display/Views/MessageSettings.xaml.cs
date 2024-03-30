using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

using WpfScreenHelper;

namespace CN.Desktop.Display.Views;

/// <summary>
/// Interaction logic for Settings.xaml
/// </summary>
public partial class MessageSettings : Window
{
    private readonly string prevServerUrl;
    public MessageSettings()
    {
        InitializeComponent();
        this.prevServerUrl = Properties.Settings.Default.ServerUrl;
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        foreach (Screen? screen in Screen.AllScreens)
        {
            _ = this.ScreensCombobox.Items.Add(screen.DeviceName);
        }

        if (string.IsNullOrWhiteSpace(Properties.Settings.Default.DisplayDeviceName))
        {
            this.ScreensCombobox.SelectedIndex = 0;
        }
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void OKBtn_Click(object sender, RoutedEventArgs e)
    {
        bool serverChanged = this.ServerUrlTextBox.Text != this.prevServerUrl;

        if (serverChanged && MessageBox.Show(
                "Alterar a URL do servidor também excluirá todos os canais e usuários configurados.\n" +
                "Tem certeza de que deseja continuar?",
                "Atenção!",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning) != MessageBoxResult.OK)
        {
            Properties.Settings.Default.ServerUrl = this.prevServerUrl;
            return;
        }
        else
        {
            Properties.Settings.Default.ServerChannels = [];
        }

        Properties.Settings.Default.Save();
        if (System.Windows.Interop.ComponentDispatcher.IsThreadModal)
            this.DialogResult = true;

        if (serverChanged)
        {
            _ = MessageBox.Show("O Aplicativo precisa ser reiniciado para se conectar ao novo servidor.\n" +
                "O app será fechado automaticamente, por favor, abra novamente e configure os canais.",
                "Aviso",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            Application.Current.Shutdown();
        }

        Close();
    }
}
