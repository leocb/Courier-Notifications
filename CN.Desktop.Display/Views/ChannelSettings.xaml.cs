using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using CN.Desktop.Display.Viewmodels;
using CN.Models.Channels;

namespace CN.Desktop.Display.Views;
/// <summary>
/// Interaction logic for ChannelSettings.xaml
/// </summary>
public partial class ChannelSettings : Window
{
    public ChannelSettings(Channel channel)
    {
        InitializeComponent();
        DataContext = new ChannelViewModel(channel, ChannelWindowMode.Creating);
    }
}
