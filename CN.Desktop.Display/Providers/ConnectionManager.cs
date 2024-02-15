using System;

using Microsoft.Win32;

namespace CN.Desktop.Display.Managers;
public static class ConnectionManager
{
    private static readonly string RegistryKeyPath = @"HKEY_CURRENT_USER\Software\Courier Notifications";
    private static readonly string RegistryValueName = @"ownerId";
    public static Guid OwnerId { get; } = InitOwnerId();

    static ConnectionManager()
    {

    }

    private static Guid InitOwnerId()
    {
        if (Registry.GetValue(RegistryKeyPath, RegistryValueName, null) is Guid guid)
            return guid;
        else
        {
            guid = Guid.NewGuid();
            Registry.SetValue(RegistryKeyPath, RegistryValueName, guid);
            return guid;
        }
    }
}
