using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

using CN.Desktop.Display.Helpers;
using CN.Desktop.Display.Providers;
using CN.Models.Channels;

namespace CN.Desktop.Display.Viewmodels;

public enum ChannelWindowMode
{
    Creating,
    Editing,
}

public class ChannelViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged(string? propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public ChannelViewModel(Guid channelId, ChannelWindowMode windowMode) : this(ChannelManager.Channels.First(c => c.Id == channelId), windowMode) { }

    public ChannelViewModel(Channel channel, ChannelWindowMode windowMode)
    {
        this.channel = channel;
        this.windowMode = windowMode;
        this.Fields = new(channel.Fields.Select(f => new FieldViewModel(f, this)));
        ChannelManager.OnStatusChanged += ChannelManager_OnStatusChanged;
    }

    private readonly ChannelWindowMode windowMode;

    private readonly Channel channel;

    private void ChannelManager_OnStatusChanged(ManagerStatus newStatus)
    {
        this.IsBusy = newStatus == ManagerStatus.Busy;
        NotifyPropertyChanged(nameof(this.IsBusy));
        NotifyPropertyChanged(nameof(this.IsNotBusy));
    }

    public bool IsBusy { get; private set; } = false;
    public bool IsNotBusy => !this.IsBusy;

    public string WindowTitle =>
        this.windowMode == ChannelWindowMode.Creating
        ? "Novo Canal"
        : "Configurar Canal";

    public string ChannelId =>
        this.windowMode == ChannelWindowMode.Creating
        ? "Crie o canal para obter um ID"
        : $"ID: {this.channel.Id}";

    public string Name
    {
        get => this.channel.Name;
        set
        {
            this.channel.Name = value;
            NotifyPropertyChanged(nameof(this.Name));
        }
    }

    public string Description
    {
        get => this.channel.Description;
        set
        {
            this.channel.Description = value;
            NotifyPropertyChanged(nameof(this.Description));
        }
    }

    public ObservableCollection<FieldViewModel> Fields { get; }

    public ICommand AddFieldCommand => new CommandHandler(AddField, true);
    public ICommand ConfirmCommand => new CommandHandler(Confirm, true);

    private void AddField()
    {
        Field field = new();
        this.channel.Fields.Add(field);
        this.Fields.Add(new(field, this));
    }

    private async void Confirm()
    {
        switch (this.windowMode)
        {
            case ChannelWindowMode.Creating:
                await ChannelManager.Create(this.channel);
                break;
            case ChannelWindowMode.Editing:
                await ChannelManager.Update(this.channel);
                break;
        }
    }
}