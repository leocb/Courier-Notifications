using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using CN.Desktop.Display.Helpers;
using CN.Desktop.Display.Providers;
using CN.Desktop.Display.Views;
using CN.Models.Roles;

namespace CN.Desktop.Display.Viewmodels;
public class RolesViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged(string? propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private readonly Guid channelId;

    public ObservableCollection<RolesItemViewModel> RolesItems { get; private set; } = [];
    public RolesViewModel(Guid channelId)
    {
        RolesManager.OnStatusChanged += RoleManager_OnStatusChanged;
        this.channelId = channelId;
        LoadRoleItemsViewModel();
        RolesManager.ChannelInfo[channelId].CollectionChanged += (s, c) => { LoadRoleItemsViewModel(); };
    }

    private void LoadRoleItemsViewModel()
    {
        ObservableCollection<AllowedSender> allowedSenders = RolesManager.ChannelInfo[this.channelId];
        this.RolesItems = new(allowedSenders
            .Where(s => s.Id != ConnectionManager.OwnerId)
            .Select(s => new RolesItemViewModel(this.channelId, s)));
        NotifyPropertyChanged(null);
    }

    private void RoleManager_OnStatusChanged(ManagerStatus newStatus)
    {
        this.IsBusy = newStatus == ManagerStatus.Busy;
        NotifyPropertyChanged(nameof(this.IsBusy));
        NotifyPropertyChanged(nameof(this.IsNotBusy));
    }

    public bool IsBusy { get; private set; } = false;
    public bool IsNotBusy => !this.IsBusy;
    public string ChannelInfo => $"Canal: {channelId}";

    public Visibility NoRoles => this.RolesItems.Count <= 0 ? Visibility.Visible : Visibility.Collapsed;

    public ICommand NewRoleCommand => new CommandHandler(() =>
    {
        Window w = new QrCodeView(this.channelId);
        _ = w.ShowDialog();
    });

    public ICommand RefreshCommand => new CommandHandler(async () =>
    {
        await RolesManager.GetAllRolesFromServer();
        LoadRoleItemsViewModel();
    });
}
