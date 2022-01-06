using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using WpfScreenHelper;

namespace CN.Desktop.Display.Views
{
    /// <summary>
    /// Interaction logic for Display.xaml
    /// </summary>
    public partial class Display : Window
    {
        public Display()
        {
            InitializeComponent();

            var displayToUse = Screen.AllScreens.FirstOrDefault(x => x.DeviceName == Properties.Settings.Default.DisplayDeviceName);
            if (displayToUse == null) displayToUse = Screen.PrimaryScreen;

            this.Left = displayToUse.Bounds.Left;
            this.Top = displayToUse.Bounds.Top;
            MainGrid.Width = displayToUse.Bounds.Width;

        }

        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            Storyboard? sb = this.FindResource("TextAnimation") as Storyboard;
            if (sb == null) return;

            Double speed;
            if (!Double.TryParse(Properties.Settings.Default.Speed, out speed)) speed = 120;
            sb.SpeedRatio = 1 / ((MessageText.ActualWidth + MainGrid.Width) / speed);
            sb.Begin();
        }

        private void Storyboard_Completed(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
