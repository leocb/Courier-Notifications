using System.ComponentModel;
using System.Windows.Input;

using CN.Desktop.Display.Helpers;
using CN.Models.Channels;

namespace CN.Desktop.Display.Viewmodels;
public class FieldViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public FieldViewModel(Field field, ChannelViewModel channelVm)
    {
        this.field = field;
        this.channelVm = channelVm;
    }

    private readonly Field field;
    private readonly ChannelViewModel channelVm;

    public ICommand RemoveFieldCommand => new CommandHandler(RemoveField, true);
    public void RemoveField() => this.channelVm.Fields.Remove(this);
}
