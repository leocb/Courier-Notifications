namespace CN.Desktop.Display.Providers;

public delegate void ManagerStatusHandler(ManagerStatus newStatus);
public enum ManagerStatus
{
    Available,
    Busy
}
