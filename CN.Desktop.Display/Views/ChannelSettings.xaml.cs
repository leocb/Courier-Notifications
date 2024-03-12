using System.Windows;

using CN.Desktop.Display.Viewmodels;
using CN.Models.Channels;

namespace CN.Desktop.Display.Views;
/// <summary>
/// Interaction logic for ChannelSettings.xaml
/// </summary>
public partial class ChannelSettings : Window
{
    public ChannelSettings(Channel channel, ChannelWindowMode mode)
    {
        ChannelViewModel vm = new(channel, mode);
        this.DataContext = vm;
        vm.CloseRequest += Close;
        InitializeComponent();
    }
}
