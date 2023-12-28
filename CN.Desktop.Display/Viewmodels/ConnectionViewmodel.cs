using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

using CN.Desktop.Display.Providers;
using CN.Models;

namespace CN.Desktop.Display.Viewmodels;

public class ConnectionViewmodel : INotifyPropertyChanged
{
    private Models.ConnectionStatus status;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged(string? propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public ConnectionViewmodel()
    {
        Properties.Settings.Default.PropertyChanged += (s, e) => { NotifyPropertyChanged(null); };
    }

    public ConnectionStatus Status
    {
        get => this.status;
        set
        {
            this.status = value;
            NotifyPropertyChanged(null);
        }
    }

    public ICommand ConnectCommand => new CommandHandler(() => ConnectClickCmdHandler(), this.ConnectButtonEnabled);

    private bool ConnectButtonEnabled => this.Status switch
    {
        ConnectionStatus.Connected or ConnectionStatus.Disconnected or ConnectionStatus.Error => true,
        _ => false,
    };

    public bool ServerConfigEnabled => this.Status switch
    {
        ConnectionStatus.Disconnected or ConnectionStatus.Error => true,
        _ => false,
    };

    public void ConnectClickCmdHandler()
    {
        switch (this.Status)
        {
            case ConnectionStatus.Connected:
                ConnectionManager.CloseAllChannels();
                break;
            case ConnectionStatus.Disconnected:
            case ConnectionStatus.Error:
                if (string.IsNullOrWhiteSpace(Properties.Settings.Default.ServerChannels))
                {
                    Properties.Settings.Default.ServerChannels = ConnectionManager.CreateRandomChannelID();
                    NotifyPropertyChanged(this.ConnectButtonText);
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

    public Brush Backcolor => this.Status switch
    {
        // https://coolors.co/fcecc9-ff8360-70b4b5-80cfab-b5c7c7
        ConnectionStatus.Connecting or ConnectionStatus.Disconnecting => new SolidColorBrush(Color.FromArgb(255, 112, 180, 181)),
        ConnectionStatus.Connected => new SolidColorBrush(Color.FromArgb(255, 128, 207, 171)),
        ConnectionStatus.Error => new SolidColorBrush(Color.FromArgb(255, 255, 131, 96)),
        ConnectionStatus.None => new SolidColorBrush(Color.FromArgb(255, 247, 247, 247)),
        ConnectionStatus.Disconnected => new SolidColorBrush(Color.FromArgb(255, 252, 236, 201)),
        _ => new SolidColorBrush(Color.FromArgb(255, 181, 199, 199)),
    };

    public string ConnectButtonText => this.Status switch
    {
        ConnectionStatus.Connected => "Desconectar",
        ConnectionStatus.Disconnected => string.IsNullOrWhiteSpace(Properties.Settings.Default.ServerChannels) ? "Criar Canal" : "Conectar",
        ConnectionStatus.Error => string.IsNullOrWhiteSpace(Properties.Settings.Default.ServerChannels) ? "Criar Canal" : "Tentar novamente",
        ConnectionStatus.Connecting => "Conectando...",
        ConnectionStatus.Disconnecting => "Desconectando...",
        ConnectionStatus.None => "Iniciando...",
        _ => "?",
    };
}