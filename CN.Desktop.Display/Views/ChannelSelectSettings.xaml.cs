using System.Windows;

using CN.Desktop.Display.Viewmodels;

namespace CN.Desktop.Display.Views;

/// <summary>
/// Interaction logic for Settings.xaml
/// </summary>
public partial class ChannelSelectSettings : Window
{
    public ChannelSelectSettings()
    {
        InitializeComponent();
        this.DataContext = new ChannelSelectViewModel();
    }
}
