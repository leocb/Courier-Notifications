using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using CN.Desktop.Display.Helpers;
using CN.Desktop.Display.Providers;

namespace CN.Desktop.Display.Viewmodels;
public class RoleQrCodeItemViewModel : INotifyPropertyChanged
{
    public event Action CloseRequest = delegate { };
    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged(string? propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private readonly Guid channelId;
    private readonly Guid authId = Guid.NewGuid();
    private string name = "";

    public RoleQrCodeItemViewModel(Guid channelId)
    {
        this.channelId = channelId;
        RolesManager.OnStatusChanged += RoleManager_OnStatusChanged;
    }
    private void RoleManager_OnStatusChanged(ManagerStatus newStatus)
    {
        this.IsBusy = newStatus == ManagerStatus.Busy;
        NotifyPropertyChanged(nameof(this.IsBusy));
        NotifyPropertyChanged(nameof(this.IsNotBusy));
    }

    public bool IsBusy { get; private set; } = false;
    public bool IsNotBusy => !this.IsBusy;

    public string Name
    {
        get => this.name;
        set
        {
            this.name = value;
            NotifyPropertyChanged(nameof(this.Name));
        }
    }

    public string URL => $"{Properties.Settings.Default.FrontUrl}/join?channel={this.channelId}&auth={this.authId}";

    public ImageSource QrCodeImage
    {
        get
        {
            return QrCode.Generate(URL);
        }
    }

    public ICommand ConfirmCommand => new CommandHandler(async () =>
    {
        try
        {
            await RolesManager.Add(this.channelId, new() { Id = this.authId, Name = this.name });
            CloseRequest.Invoke();
        }
        catch (Exception e)
        {
            await DialogHelper.ShowMessage(e.Message, "Qr");
        }
    });
    public ICommand CopyCommand => new CommandHandler(() =>
    {
        Clipboard.SetText(URL);
    });
}
