using System.Windows;
using System.Windows.Media;

using CN.Desktop.Display.Helpers;
using CN.Desktop.Display.Providers;

using MaterialDesignColors;

using MaterialDesignThemes.Wpf;

namespace CN.Desktop.Display.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class Main : Window
{
    public Main()
    {
        InitializeComponent();
        ThemeHelper.SetTheme();
    }

    private void ConfigBtn_Click(object sender, RoutedEventArgs e)
    {
        Settings config = new();
        _ = config.ShowDialog();
    }

    private void Window_ContentRendered(object sender, System.EventArgs e) => ConnectionManager.OpenAllChannels();
}
