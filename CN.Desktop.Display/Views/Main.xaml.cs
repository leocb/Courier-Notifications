using System.ComponentModel;
using System.Windows;

using CN.Desktop.Display.Viewmodels;

namespace CN.Desktop.Display.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class Main : Window
{
    private readonly MainViewmodel vm = new();
    public Main()
    {
        InitializeComponent();
        this.DataContext = this.vm;
    }

    private void ConfigBtn_Click(object sender, RoutedEventArgs e)
    {
        MessageSettings config = new();
        _ = config.ShowDialog();
    }

    private void ChannelConfigBtn_Click(object sender, RoutedEventArgs e)
    {
        ChannelSelectSettings config = new();
        _ = config.ShowDialog();
    }

    private void CloseBtn_Click(object sender, RoutedEventArgs e) => App.Current.Shutdown(0);

    private async void Window_ContentRendered(object sender, System.EventArgs e) => await this.vm.ConnectToAll();

    // Minimize to system tray when application is closed.
    protected override void OnClosing(CancelEventArgs e)
    {
        // setting cancel to true will cancel the close request
        // so the application is not closed
        e.Cancel = true;
        base.OnClosing(e);
    }
}
