using CN.Desktop.Display.Providers;
using CN.Models.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CN.Desktop.Display.Viewmodels
{
    public class MessageViewmodel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public Message Message { get; set; }

        public MessageViewmodel(Message message)
        {
            Message = message;
        }

        public string Text => Message.Text;
        public string DateTimeText => Message.Date.ToString("dd/MM/yyyy hh:mm:ss");
        public string From => Message.FromName;
        public string Info => $"{DateTimeText} - {From}";
        public MessageStatus Status
        {
            get
            {
                return Message.Status;
            }
            set
            {
                Message.Status = value;
                NotifyPropertyChanged(nameof(Status));
                NotifyPropertyChanged(nameof(ButtonText));
                NotifyPropertyChanged(nameof(ButtonEnabled));
                NotifyPropertyChanged(nameof(Backcolor));
            }
        }

        public ICommand ClickCommand
        {
            get
            {
                return new CommandHandler(() => ButtonClickCmdHandler(), ButtonEnabled);
            }
        }

        public Brush Backcolor
        {
            get
            {
                switch (Status)
                {
                    // https://coolors.co/fcecc9-ff8360-70b4b5-80cfab-b5c7c7
                    case MessageStatus.Waiting:
                        return new SolidColorBrush(Color.FromArgb(255, 252, 236, 201));
                    case MessageStatus.BeingDisplayed:
                        return new SolidColorBrush(Color.FromArgb(255, 112, 180, 181));
                    case MessageStatus.Canceled:
                        return new SolidColorBrush(Color.FromArgb(255, 181, 199, 199));
                    case MessageStatus.OK:
                        return new SolidColorBrush(Color.FromArgb(255, 128, 207, 171));
                    default:
                        return new SolidColorBrush(Color.FromArgb(255, 255, 131, 96));
                }
            }
        }

        private bool ButtonEnabled
        {
            get
            {
                switch (Status)
                {
                    case MessageStatus.None:
                    case MessageStatus.Waiting:
                    case MessageStatus.BeingDisplayed:
                    case MessageStatus.Canceled:
                    case MessageStatus.OK:
                        return true;
                    default:
                        return false;
                }
            }
        }


        public void ButtonClickCmdHandler()
        {
            switch (Status)
            {
                case MessageStatus.None:
                case MessageStatus.Waiting: // Block message
                    Status = MessageStatus.Canceled;
                    break;
                case MessageStatus.BeingDisplayed: // Stop current display
                    MessageDisplayManager.StopCurrentMessageDisplay();
                    break;
                case MessageStatus.Canceled: // Allow message
                    MessageDisplayManager.RestoreMessageToQueue(this);
                    break;
                case MessageStatus.OK: // Duplicate message
                    MessageDisplayManager.AddMessage(new Message()
                    {
                        FromName = $"{Message.FromName} [Dup]",
                        Text = Message.Text,
                        Date = DateTime.Now,
                    });
                    break;
                default:
                    return;
            }
        }

        public string ButtonText
        {
            get
            {
                switch (Status)
                {
                    case MessageStatus.None:
                    case MessageStatus.Waiting:
                        return "Na fila";
                    case MessageStatus.BeingDisplayed:
                        return "Cancelar";
                    case MessageStatus.Canceled:
                        return "Mostrar";
                    case MessageStatus.OK:
                        return "Duplicar";
                    default:
                        return "?";
                }
            }
        }

    }
}
