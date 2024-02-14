using System;

using CN.Desktop.Display.Managers;
using CN.Models.Messages;

namespace CN.Desktop.Display.Providers;
public static class SystemMessage
{
    public static void Error(string message)
    {
        MessageQueue.Add(new Message()
        {
            Text = message,
            Date = DateTime.Now,
            From = ConnectionManager.OwnerId,
            Status = MessageStatus.Error
        });
    }

    public static void Info(string message)
    {
        MessageQueue.Add(new Message()
        {
            Text = message,
            Date = DateTime.Now,
            From = ConnectionManager.OwnerId,
            Status = MessageStatus.Info
        });
    }
}
