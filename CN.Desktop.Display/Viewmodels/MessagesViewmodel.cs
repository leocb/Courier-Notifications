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
    public static class MessagesViewmodel
    {
        public static ObservableCollection<MessageViewmodel> Messages { get; set; } = new ObservableCollection<MessageViewmodel>();
        public static BlockingCollection<MessageViewmodel> DisplayMessages { get; set; } = new BlockingCollection<MessageViewmodel>();

        private static Window? currentDisplay;

        static MessagesViewmodel()
        {
            var displayTask = new Thread(() =>
            {
                while (true)
                {
                    var message = DisplayMessages.Take();
                    if (message.Status != MessageStatus.Waiting) continue;
                    message.Status = MessageStatus.BeingDisplayed;

                    currentDisplay = new Views.Display(message.Text);
                    var completed = currentDisplay.ShowDialog();
                    currentDisplay = null;

                    if (completed ?? false)
                        message.Status = MessageStatus.OK;
                    else
                        message.Status = MessageStatus.Canceled;
                }
            });

            displayTask.SetApartmentState(ApartmentState.STA);
            displayTask.IsBackground = true;
            displayTask.Start();

            AddMessage(new Message() { Date = DateTime.Now, FromName = "Leonardo Bottaro", Text = "Mensagem marota" });

        }

        public static void AddMessage(Message message)
        {
            var msgVM = new MessageViewmodel(message);
            msgVM.Status = MessageStatus.Waiting;
            DisplayMessages.Add(msgVM);
            Messages.Insert(0, msgVM);

            while (Messages.Count > 50)
                Messages.RemoveAt(50);

        }

        public static void StopCurrentMessageDisplay()
        {
            currentDisplay?.Dispatcher.Invoke(() => { currentDisplay.Close(); });
        }
    }

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
                case MessageStatus.Waiting:
                    Status = MessageStatus.Canceled;
                    break;
                case MessageStatus.BeingDisplayed:
                    MessagesViewmodel.StopCurrentMessageDisplay();
                    break;
                case MessageStatus.Canceled:
                    Status = MessageStatus.Waiting;
                    MessagesViewmodel.DisplayMessages.Add(this);
                    break;
                case MessageStatus.OK:
                    MessagesViewmodel.AddMessage(new Message()
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
                        return "Esperando";
                    case MessageStatus.BeingDisplayed:
                        return "Cancelar";
                    case MessageStatus.Canceled:
                        return "Permitir";
                    case MessageStatus.OK:
                        return "Duplicar";
                    default:
                        return "?";
                }
            }
        }

    }
}
