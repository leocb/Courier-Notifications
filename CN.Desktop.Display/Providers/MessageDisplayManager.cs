using CN.Desktop.Display.Viewmodels;
using CN.Models.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;

namespace CN.Desktop.Display.Providers
{
    public static class MessageDisplayManager
    {
        public static ObservableCollection<MessageViewmodel> Messages { get; set; } = new ObservableCollection<MessageViewmodel>();
        private static BlockingCollection<MessageViewmodel> DisplayMessages { get; set; } = new BlockingCollection<MessageViewmodel>();

        private static Window? currentDisplay;

        static MessageDisplayManager()
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

        public static void RestoreMessageToQueue(MessageViewmodel message)
        {
            message.Status = MessageStatus.Waiting;
            DisplayMessages.Add(message);
        }

        public static void StopCurrentMessageDisplay()
        {
            currentDisplay?.Dispatcher.Invoke(() => { currentDisplay.Close(); });
        }
    }
}
