using CN.Desktop.Display.Providers;
using System.Windows;

namespace CN.Desktop.Display.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
        }

        private void ConfigBtn_Click(object sender, RoutedEventArgs e)
        {
            var config = new Settings();
            config.ShowDialog();
        }

        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            ConnectionManager.OpenAllChannels();
        }
    }
}
