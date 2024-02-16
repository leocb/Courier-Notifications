using System.Windows.Controls;

namespace CN.Desktop.Display.Views;
public partial class MessageDialog : UserControl
{
    public string Message { get; }
    public MessageDialog(string message)
    {
        this.Message = message;
        InitializeComponent();
    }
}
