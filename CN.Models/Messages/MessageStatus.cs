using System;
using System.Collections.Generic;
using System.Text;

namespace CN.Models.Messages
{
    public enum MessageStatus
    {
        None,
        Failed,
        Failed_CentralNotFound,
        Failed_SpaceNotFound,
        Failed_InvalidField,
        Failed_NoPermission,
        Waiting,
        BeingDisplayed,
        Canceled,
        OK
    }
}
