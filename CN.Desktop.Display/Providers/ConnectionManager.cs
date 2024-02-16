using System;

using Microsoft.Win32;

namespace CN.Desktop.Display.Providers;
public static class ConnectionManager
{
    private static readonly string RegistryKeyPath = @"HKEY_CURRENT_USER\Software\Courier Notifications";
    private static readonly string RegistryValueName = @"ownerId";
    public static Guid OwnerId { get; } = InitOwnerId();

    private static Guid InitOwnerId()
    {
        if (Registry.GetValue(RegistryKeyPath, RegistryValueName, null) is string registryValue
            && Guid.TryParse(registryValue, out Guid ownerIdRegistry))
        {
            return ownerIdRegistry;
        }

        Guid guid = Guid.NewGuid();
        Registry.SetValue(RegistryKeyPath, RegistryValueName, guid);
        return guid;
    }
}
