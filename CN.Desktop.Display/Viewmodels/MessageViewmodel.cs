using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

using CN.Desktop.Display.Providers;
using CN.Models.Messages;

namespace CN.Desktop.Display.Viewmodels;

public class MessageViewmodel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public Message Message { get; set; }

    public MessageViewmodel(Message message)
    {
        this.Message = message;
    }

    public string Text => this.Message.Text;
    public string DateTimeText => this.Message.Date.ToString("dd/MM/yyyy hh:mm:ss");
    public string From => this.Message.FromName;
    public string Info => $"{this.DateTimeText} - {this.From}";
    public MessageStatus Status
    {
        get => this.Message.Status;
        set
        {
            this.Message.Status = value;
            NotifyPropertyChanged(nameof(this.Status));
            NotifyPropertyChanged(nameof(this.ButtonText));
            NotifyPropertyChanged(nameof(this.ButtonEnabled));
            NotifyPropertyChanged(nameof(this.Backcolor));
        }
    }

    public ICommand ClickCommand => new CommandHandler(() => ButtonClickCmdHandler(), this.ButtonEnabled);

    public Brush Backcolor => this.Status switch
    {
        // https://coolors.co/fcecc9-ff8360-70b4b5-80cfab-b5c7c7
        MessageStatus.Waiting => new SolidColorBrush(Color.FromArgb(255, 252, 236, 201)),
        MessageStatus.BeingDisplayed => new SolidColorBrush(Color.FromArgb(255, 112, 180, 181)),
        MessageStatus.Canceled => new SolidColorBrush(Color.FromArgb(255, 181, 199, 199)),
        MessageStatus.Info => new SolidColorBrush(Color.FromArgb(255, 247, 247, 247)),
        MessageStatus.OK => new SolidColorBrush(Color.FromArgb(255, 128, 207, 171)),
        _ => new SolidColorBrush(Color.FromArgb(255, 255, 131, 96)),
    };

    private bool ButtonEnabled => this.Status switch
    {
        MessageStatus.None or MessageStatus.Waiting or MessageStatus.BeingDisplayed or MessageStatus.Canceled or MessageStatus.OK => true,
        _ => false,
    };

    public void ButtonClickCmdHandler()
    {
        switch (this.Status)
        {
            case MessageStatus.None:
            case MessageStatus.Waiting: // Block message
                this.Status = MessageStatus.Canceled;
                break;
            case MessageStatus.BeingDisplayed: // Stop current display
                MessageDisplayManager.StopCurrentMessageDisplay();
                break;
            case MessageStatus.Canceled: // Allow message
                MessageDisplayManager.RestoreMessageToQueue(this);
                break;
            case MessageStatus.OK: // Duplicate message
                MessageDisplayManager.AddDisplayMessage(new Message()
                {
                    FromName = $"{this.Message.FromName} [Dup]",
                    Text = this.Message.Text,
                    Date = DateTime.Now,
                });
                break;
            default:
                return;
        }
    }

    public string ButtonText => this.Status switch
    {
        MessageStatus.None or MessageStatus.Waiting => "Na fila",
        MessageStatus.BeingDisplayed => "Cancelar",
        MessageStatus.Canceled => "Mostrar",
        MessageStatus.OK => "Duplicar",
        MessageStatus.Info => "Info",
        _ => ":(",
    };
}
