using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

using CN.Desktop.Display.Helpers;
using CN.Desktop.Display.Managers;
using CN.Models;

namespace CN.Desktop.Display.Viewmodels;

public class MainViewmodel : INotifyPropertyChanged
{
    private ConnectionStatus status;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged(string? propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public MainViewmodel()
    {
        SocketManager.OnStatusChanged += SocketManager_OnStatusChanged;
    }

    private void SocketManager_OnStatusChanged(ConnectionStatus status)
    {
        Status = status;
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

    public ICommand ConnectCommand => new CommandHandler(ConnectClickCmdHandler, this.ConnectButtonEnabled);

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

    public async Task ConnectToAll()
    {
        await SocketManager.OpenAll();
    }
    public async Task CloseAll()
    {
        await SocketManager.CloseAllChannels();
    }

    public async void ConnectClickCmdHandler()
    {
        switch (this.Status)
        {
            case ConnectionStatus.Connected:
                await CloseAll();
                break;
            default:
                await ConnectToAll();
                break;
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
        ConnectionStatus.Disconnected => "Conectar",
        ConnectionStatus.Error => "Tentar novamente",
        ConnectionStatus.Connecting => "Conectando...",
        ConnectionStatus.Disconnecting => "Desconectando...",
        ConnectionStatus.None => "Iniciando...",
        _ => "?",
    };
}