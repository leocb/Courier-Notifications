using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using CN.Desktop.Display.Helpers;
using CN.Desktop.Display.Providers;
using CN.Desktop.Display.Views;

namespace CN.Desktop.Display.Viewmodels;
public class ChannelSelectViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged(string? propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public ObservableCollection<ChannelSelectItemViewModel> ChannelsItems { get; private set; } = [];
    public ChannelSelectViewModel()
    {
        LoadChannelItemsViewModel();
        ChannelManager.Channels.CollectionChanged += (s, c) => { LoadChannelItemsViewModel(); };
        ChannelManager.OnStatusChanged += ChannelManager_OnStatusChanged;
    }

    private void LoadChannelItemsViewModel()
    {
        this.ChannelsItems = new(ChannelManager.Channels.Select(c => new ChannelSelectItemViewModel(c)));
        NotifyPropertyChanged(null);
    }

    private void ChannelManager_OnStatusChanged(ManagerStatus newStatus)
    {
        this.IsBusy = newStatus == ManagerStatus.Busy;
        NotifyPropertyChanged(nameof(this.IsBusy));
        NotifyPropertyChanged(nameof(this.IsNotBusy));
    }

    public bool IsBusy { get; private set; } = false;
    public bool IsNotBusy => !this.IsBusy;

    public Visibility NoChannels => ChannelManager.Channels.Count <= 0 ? Visibility.Visible : Visibility.Collapsed;

    public ICommand NewChannelCommand => new CommandHandler(() =>
    {
        Window w = new ChannelSettings(new(), ChannelWindowMode.Creating);
        _ = w.ShowDialog();
    });

    public ICommand RefreshCommand => new CommandHandler(async () =>
    {
        await ChannelManager.GetAllChannelsFromServer();
        LoadChannelItemsViewModel();
    });
}
