namespace CN.Web.App.Providers;

public delegate void ManagerStatusHandler(ManagerStatus newStatus);
public enum ManagerStatus
{
    Available,
    Busy
}
