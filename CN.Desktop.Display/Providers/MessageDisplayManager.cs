using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;

using CN.Desktop.Display.Viewmodels;
using CN.Models.Messages;

namespace CN.Desktop.Display.Providers;

public static class MessageDisplayManager
{
    public static ObservableCollection<MessageViewmodel> Messages { get; set; } = new ObservableCollection<MessageViewmodel>();
    private static BlockingCollection<MessageViewmodel> DisplayMessages { get; set; } = new BlockingCollection<MessageViewmodel>();

    private static Window? currentDisplay;

    static MessageDisplayManager()
    {
        Thread displayTask = new(MessageQueueDisplayThread());
        displayTask.SetApartmentState(ApartmentState.STA);
        displayTask.IsBackground = true;
        displayTask.Start();
    }

    private static ThreadStart MessageQueueDisplayThread()
    {
        while (true)
        {
            MessageViewmodel message = DisplayMessages.Take();
            if (message.Status != MessageStatus.Waiting)
                continue;

            message.Status = MessageStatus.BeingDisplayed;

            currentDisplay = new Views.Display(message.Text);
            bool? completed = currentDisplay.ShowDialog();
            currentDisplay = null;

            message.Status = completed ?? false ? MessageStatus.OK : MessageStatus.Canceled;
        }
    }

    public static void AddDisplayMessage(Message message)
    {
        MessageViewmodel msgVM = new(message)
        {
            Status = MessageStatus.Waiting
        };

        Application.Current.Dispatcher.Invoke(delegate
        {
            DisplayMessages.Add(msgVM);
            Messages.Insert(0, msgVM);
        });
    }

    public static void AddInternalMessage(Message message, MessageStatus status = MessageStatus.Failed)
    {
        MessageViewmodel msgVM = new(message)
        {
            Status = status
        };

        Application.Current.Dispatcher.Invoke(delegate
        {
            Messages.Insert(0, msgVM);
        });
    }

    public static void RestoreMessageToQueue(MessageViewmodel message)
    {
        message.Status = MessageStatus.Waiting;
        DisplayMessages.Add(message);
    }

    public static void StopCurrentMessageDisplay() => currentDisplay?.Dispatcher.Invoke(() => { currentDisplay.Close(); });
}
