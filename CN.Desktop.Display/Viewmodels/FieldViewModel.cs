using System.ComponentModel;
using System.Windows.Input;

using CN.Desktop.Display.Helpers;
using CN.Models.Channels;

namespace CN.Desktop.Display.Viewmodels;
public class FieldViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged(string? propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public FieldViewModel(Field field, ChannelViewModel channelVm)
    {
        this.Field = field;
        this.channelVm = channelVm;
    }

    public Field Field { get; }
    private readonly ChannelViewModel channelVm;

    public ICommand RemoveFieldCommand => new CommandHandler(RemoveField, true);
    public void RemoveField() => this.channelVm.RemoveField(this);

    public string Header => string.IsNullOrEmpty(this.Field.Name) ? "Novo Campo" : this.Field.Name;

    public string Name
    {
        get => this.Field.Name;
        set
        {
            this.Field.Name = value;
            NotifyPropertyChanged(nameof(this.Name));
            NotifyPropertyChanged(nameof(this.Header));
        }
    }

    public bool IsNumeric
    {
        get => this.Field.IsNumeric;
        set
        {
            this.Field.IsNumeric = value;
            NotifyPropertyChanged(nameof(this.IsNumeric));
        }
    }

    public string RegexValidation
    {
        get => this.Field.RegexValidation;
        set
        {
            this.Field.RegexValidation = value;
            NotifyPropertyChanged(nameof(this.RegexValidation));
        }
    }

    public string TextBeforeValue
    {
        get => this.Field.TextBeforeValue;
        set
        {
            this.Field.TextBeforeValue = value;
            NotifyPropertyChanged(nameof(this.TextBeforeValue));
        }
    }

    public string TextAfterValue
    {
        get => this.Field.TextAfterValue;
        set
        {
            this.Field.TextAfterValue = value;
            NotifyPropertyChanged(nameof(this.TextAfterValue));
        }
    }

    public string RegexForAlternate
    {
        get => this.Field.RegexForAlternate;
        set
        {
            this.Field.RegexForAlternate = value;
            NotifyPropertyChanged(nameof(this.RegexForAlternate));
        }
    }

    public string TextBeforeValueAlternate
    {
        get => this.Field.TextBeforeValueAlternate;
        set
        {
            this.Field.TextBeforeValueAlternate = value;
            NotifyPropertyChanged(nameof(this.TextBeforeValueAlternate));
        }
    }

    public string TextAfterValueAlternate
    {
        get => this.Field.TextAfterValueAlternate;
        set
        {
            this.Field.TextAfterValueAlternate = value;
            NotifyPropertyChanged(nameof(this.TextAfterValueAlternate));
        }
    }
}
