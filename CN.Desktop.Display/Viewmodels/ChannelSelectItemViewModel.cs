using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using CN.Desktop.Display.Helpers;
using CN.Desktop.Display.Providers;
using CN.Desktop.Display.Views;
using CN.Models.Channels;

namespace CN.Desktop.Display.Viewmodels;
public class ChannelSelectItemViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action SomethingChanged = delegate { };
    private void NotifyPropertyChanged(string? propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private readonly Channel channel;
    public ChannelSelectItemViewModel(Channel channel)
    {
        this.channel = channel;
    }

    public string? Name => this.channel.Name;
    public string? Id => this.channel.Id.ToString();

    public ICommand EditCommand => new CommandHandler(() =>
    {
        Window w = new ChannelSettings(this.channel, ChannelWindowMode.Editing);
        _ = w.ShowDialog();
        NotifyPropertyChanged(null);
        SomethingChanged.Invoke();
    });

    public ICommand DeleteCommand => new CommandHandler(async () =>
    {
        try
        {
            if (!await DialogHelper.ShowConfirmationMessage($"Certeza que deseja excluir o canal {this.channel.Name}?", "ChannelSelect"))
                return;

            await ChannelManager.Delete(this.channel.Id);
            SomethingChanged.Invoke();
        }
        catch (Exception e)
        {
            await DialogHelper.ShowMessage(e.Message, "ChannelSelect");
        }
    });

    public ICommand EditUsersCommand => new CommandHandler(() =>
    {

        Window w = new ChannelUsers(this.channel);
        _ = w.ShowDialog();
    });

}
