using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

using WpfScreenHelper;

namespace CN.Desktop.Display.Views;

/// <summary>
/// Interaction logic for Settings.xaml
/// </summary>
public partial class Settings : Window
{
    public Settings()
    {
        InitializeComponent();
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
        Properties.Settings.Default.Save();
        if (System.Windows.Interop.ComponentDispatcher.IsThreadModal)
            this.DialogResult = true;
        Close();
    }
}
