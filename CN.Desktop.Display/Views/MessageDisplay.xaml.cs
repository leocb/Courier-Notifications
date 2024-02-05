using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

using WpfScreenHelper;

namespace CN.Desktop.Display.Views;

/// <summary>
/// Interaction logic for Display.xaml
/// </summary>
public partial class MessageDisplay : Window
{
    public MessageDisplay(string Text)
    {
        InitializeComponent();

        this.MessageText.Content = Text;

        Screen? displayToUse = Screen.AllScreens.FirstOrDefault(x => x.DeviceName == Properties.Settings.Default.DisplayDeviceName);
        displayToUse ??= Screen.PrimaryScreen;

        this.Left = displayToUse.Bounds.Left;
        this.Top = displayToUse.Bounds.Top;
        this.MainGrid.Width = displayToUse.Bounds.Width;
    }

    private void Window_ContentRendered(object sender, System.EventArgs e)
    {
        if (FindResource("TextAnimation") is Storyboard sb)
        {
            if (!double.TryParse(Properties.Settings.Default.Speed, out double speed))
                speed = 120;

            sb.SpeedRatio = 1 / ((this.MessageText.ActualWidth + this.MainGrid.Width) / speed);
            sb.Begin();
        }
    }

    private void Storyboard_Completed(object sender, System.EventArgs e)
    {
        if (this.IsVisible)
            this.DialogResult = true;
        Close();
    }
}
