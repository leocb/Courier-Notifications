using System.Windows;
using System.Windows.Controls;

namespace CN.Desktop.Display.Views;
public partial class MessageDialog : UserControl
{
    public string Message { get; }
    public Visibility ConfirmMode { get; }
    public Visibility MessageMode { get; }
    public MessageDialog(string message, bool confirm)
    {
        this.Message = message;
        this.ConfirmMode = confirm ? Visibility.Visible : Visibility.Collapsed;
        this.MessageMode = !confirm ? Visibility.Visible : Visibility.Collapsed;
        InitializeComponent();
    }
}
