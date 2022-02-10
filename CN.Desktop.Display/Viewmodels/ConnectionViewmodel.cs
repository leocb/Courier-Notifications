using CN.Desktop.Display.Providers;
using CN.Models;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace CN.Desktop.Display.Viewmodels
{
    public class ConnectionViewmodel : INotifyPropertyChanged
    {
        private Models.ConnectionStatus status;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged(string? propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public ConnectionViewmodel()
        {
            Properties.Settings.Default.PropertyChanged += (s, e) => { NotifyPropertyChanged(null); };
        }

        public ConnectionStatus Status
        {
            get => status;
            set
            {
                status = value;
                NotifyPropertyChanged(null);
            }
        }

        public ICommand ConnectCommand => new CommandHandler(() => ConnectClickCmdHandler(), ConnectButtonEnabled);

        private bool ConnectButtonEnabled
        {
            get
            {
                switch (Status)
                {
                    case ConnectionStatus.Connected:
                    case ConnectionStatus.Disconnected:
                    case ConnectionStatus.Error:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public bool ServerConfigEnabled
        {
            get
            {
                switch (Status)
                {
                    case ConnectionStatus.Disconnected:
                    case ConnectionStatus.Error:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public void ConnectClickCmdHandler()
        {
            switch (Status)
            {
                case ConnectionStatus.Connected:
                    ConnectionManager.CloseAllChannels();
                    break;
                case ConnectionStatus.Disconnected:
                case ConnectionStatus.Error:
                    if (string.IsNullOrWhiteSpace(Properties.Settings.Default.ServerChannels))
                    {
                        Properties.Settings.Default.ServerChannels = ConnectionManager.CreateRandomChannelID();
                        NotifyPropertyChanged(ConnectButtonText);
                        break;
                    }
                    else
                    {
                        ConnectionManager.OpenAllChannels();
                    }
                    break;
                default:
                    return;
            }
        }

        public Brush Backcolor
        {
            get
            {
                switch (Status)
                {
                    // https://coolors.co/fcecc9-ff8360-70b4b5-80cfab-b5c7c7
                    case ConnectionStatus.Connecting:
                    case ConnectionStatus.Disconnecting:
                        return new SolidColorBrush(Color.FromArgb(255, 112, 180, 181));
                    case ConnectionStatus.Connected:
                        return new SolidColorBrush(Color.FromArgb(255, 128, 207, 171));
                    case ConnectionStatus.Error:
                        return new SolidColorBrush(Color.FromArgb(255, 255, 131, 96));
                    case ConnectionStatus.None:
                        return new SolidColorBrush(Color.FromArgb(255, 247, 247, 247));
                    case ConnectionStatus.Disconnected:
                        return new SolidColorBrush(Color.FromArgb(255, 252, 236, 201));
                    default:
                        return new SolidColorBrush(Color.FromArgb(255, 181, 199, 199));
                }
            }
        }

        public string ConnectButtonText
        {
            get
            {
                switch (Status)
                {
                    case ConnectionStatus.Connected:
                        return "Desconectar";
                    case ConnectionStatus.Disconnected:
                        if (string.IsNullOrWhiteSpace(Properties.Settings.Default.ServerChannels))
                            return "Criar Canal";
                        else
                            return "Conectar";
                    case ConnectionStatus.Error:
                        if (string.IsNullOrWhiteSpace(Properties.Settings.Default.ServerChannels))
                            return "Criar Canal";
                        else
                            return "Tentar novamente";
                    case ConnectionStatus.Connecting:
                        return "Conectando...";
                    case ConnectionStatus.Disconnecting:
                        return "Desconectando...";
                    case ConnectionStatus.None:
                        return "Iniciando...";
                    default:
                        return "?";
                }
            }
        }
    }
}