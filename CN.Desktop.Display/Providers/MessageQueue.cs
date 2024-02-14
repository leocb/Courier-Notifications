using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;

using CN.Desktop.Display.Viewmodels;
using CN.Models.Messages;

namespace CN.Desktop.Display.Providers;

public static class MessageQueue
{
    public static ObservableCollection<MessageViewmodel> Messages { get; } = [];
    private static BlockingCollection<MessageViewmodel> DisplayMessages { get; } = [];

    private static Window? MessageWindow;

    static MessageQueue()
    {
        Thread displayTask = new(new ThreadStart(() => { MessageQueueDisplayThread(CancellationToken.None); }));
        displayTask.SetApartmentState(ApartmentState.STA);
        displayTask.IsBackground = true;
        displayTask.Start();
    }

    private static void MessageQueueDisplayThread(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            MessageViewmodel vm = DisplayMessages.Take(ct);
            if (vm.Status != MessageStatus.Queued)
                continue;

            vm.Status = MessageStatus.BeingDisplayed;

            MessageWindow = new Views.MessageDisplay(vm.Message.Text);

            // this will block until the current message window is closed
            bool? completed = MessageWindow.ShowDialog();
            MessageWindow = null;

            vm.Status = completed ?? false ? MessageStatus.OK : MessageStatus.Canceled;
        }
    }

    public static void Add(Message message)
    {
        message.Date = System.DateTime.Now;

        MessageViewmodel msgVM = new(message);

        Application.Current.Dispatcher.Invoke(delegate
        {
            if (message.Status == MessageStatus.Queued)
                DisplayMessages.Add(msgVM);

            Messages.Insert(0, msgVM);
        });
    }

    public static void DuplicateMessageToQueue(MessageViewmodel vm)
    {
        Add(new()
        {
            Text = vm.Message.Text,
            From = vm.Message.From,
            Status = MessageStatus.Queued
        });
    }

    public static void StopCurrentMessageDisplay() => MessageWindow?.Dispatcher.Invoke(() => { MessageWindow.Close(); });
}
