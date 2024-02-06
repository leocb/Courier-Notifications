using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

using CN.Desktop.Display.Helpers;

using WpfScreenHelper;

namespace CN.Desktop.Display.Views;

/// <summary>
/// Interaction logic for Settings.xaml
/// </summary>
public partial class ChannelSelectSettings : Window
{
    public ChannelSelectSettings()
    {
        InitializeComponent();
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        
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
