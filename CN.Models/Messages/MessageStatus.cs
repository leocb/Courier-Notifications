namespace CN.Models.Messages;

public enum MessageStatus
{
    None,
    Failed,
    Failed_CentralNotFound,
    Failed_SpaceNotFound,
    Failed_InvalidField,
    Failed_NoPermission,
    Info,
    Waiting,
    BeingDisplayed,
    Canceled,
    OK
}
