using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

using CN.Desktop.Display.Helpers;
using CN.Desktop.Display.Providers;
using CN.Models.Messages;

using MaterialDesignThemes.Wpf;

namespace CN.Desktop.Display.Viewmodels;

public class MessageViewmodel(Message message) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public Message Message { get; set; } = message;
    public string Text => this.Message.Text;

    public string StatusText => this.Status switch
    {
        MessageStatus.None or MessageStatus.Queued => "NA FILA",
        MessageStatus.BeingDisplayed => "EM EXIBIÇÃO",
        MessageStatus.Canceled => "CANCELADO",
        MessageStatus.OK => "OK",
        MessageStatus.Info => "INFORMAÇÃO",
        _ => "ERRO",
    };
    public string DateTimeText => this.Message.Date.ToString("HH:mm:ss");
    public string FromName => "";
    public string ChannelName => "";

    public PackIconKind Icon => this.Status switch
    {
        MessageStatus.Queued => PackIconKind.DotsHorizontal,
        MessageStatus.BeingDisplayed => PackIconKind.ArrowUpBoldCircle,
        MessageStatus.Canceled => PackIconKind.Cancel,
        MessageStatus.Info => PackIconKind.InformationVariant,
        MessageStatus.Error => PackIconKind.Fire,
        MessageStatus.OK => PackIconKind.Check,
        MessageStatus.None => PackIconKind.CloudQuestion,
        _ => PackIconKind.CloudQuestion
    };

    public string Info => $"{this.StatusText}  •  {this.DateTimeText}" +
        (!string.IsNullOrEmpty(this.ChannelName) ? $"  •  {this.ChannelName}" : "") +
        (!string.IsNullOrEmpty(this.FromName) ? $"  •  {this.FromName}" : "");

    public MessageStatus Status
    {
        get => this.Message.Status;
        set
        {
            this.Message.Status = value;
            NotifyPropertyChanged(nameof(this.Status));
            NotifyPropertyChanged(nameof(this.Info));
            NotifyPropertyChanged(nameof(this.Icon));
            NotifyPropertyChanged(nameof(this.Backcolor));
        }
    }

    public ICommand ClickCommand => new CommandHandler(ButtonClickCmdHandler, true);

    public Brush Backcolor => this.Status switch
    {
        // https://coolors.co/fcecc9-ff8360-70b4b5-80cfab-b5c7c7
        MessageStatus.Queued => new SolidColorBrush(Color.FromArgb(255, 252, 236, 201)),
        MessageStatus.BeingDisplayed => new SolidColorBrush(Color.FromArgb(255, 112, 180, 181)),
        MessageStatus.Canceled => new SolidColorBrush(Color.FromArgb(255, 181, 199, 199)),
        MessageStatus.Info => new SolidColorBrush(Color.FromArgb(255, 247, 247, 247)),
        MessageStatus.OK => new SolidColorBrush(Color.FromArgb(255, 128, 207, 171)),
        _ => new SolidColorBrush(Color.FromArgb(255, 255, 131, 96)),
    };

    public void ButtonClickCmdHandler()
    {
        switch (this.Status)
        {
            case MessageStatus.Queued: // Block message
                this.Status = MessageStatus.Canceled;
                break;
            case MessageStatus.BeingDisplayed: // Stop current display
                MessageQueue.StopCurrentMessageDisplay();
                break;
            case MessageStatus.Canceled: // Allow message
            case MessageStatus.OK: // Duplicate message
                MessageQueue.DuplicateMessageToQueue(this);
                break;
            default:
                return;
        }
    }
}
