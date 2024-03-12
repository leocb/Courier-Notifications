using System.Windows;

using CN.Desktop.Display.Viewmodels;
using CN.Models.Channels;

namespace CN.Desktop.Display.Views;
/// <summary>
/// Interaction logic for ChannelUsers.xaml
/// </summary>
public partial class ChannelUsers : Window
{
    public ChannelUsers(Channel channel)
    {
        this.DataContext = new RolesViewModel(channel.Id);
        InitializeComponent();
    }
}
