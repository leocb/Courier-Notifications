using System.Windows;

using CN.Desktop.Display.Providers;
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
        ChannelSettings config = new(ChannelManager.Channels[0], ChannelWindowMode.Editing);
        _ = config.ShowDialog();
    }

    private async void Window_ContentRendered(object sender, System.EventArgs e) => await this.vm.ConnectToAll();
}
