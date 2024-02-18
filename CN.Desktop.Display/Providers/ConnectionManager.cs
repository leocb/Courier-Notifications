using System;

namespace CN.Desktop.Display.Providers;
public static class ConnectionManager
{
    public static Guid OwnerId { get; } = InitOwnerId();

    private static Guid InitOwnerId()
    {
        if (Properties.Settings.Default.OwnerId.Equals(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)))
        {
            Properties.Settings.Default.OwnerId = Guid.NewGuid();
            Properties.Settings.Default.Save();
        }

        return Properties.Settings.Default.OwnerId;
    }
}
