using System;
using System.Collections.Generic;

using CN.Models.Exceptions;

namespace CN.Models.Server;

public class ChannelRoles

{
    public Guid Id { get; set; } // ChannelId
    public Guid Owner { get; set; }
    public List<AllowedSender> AllowedSenders { get; set; } = [];

    public void ValidateOwner(Guid ownerId)
    {
        if (this.Owner != ownerId)
            throw new CourierException("Unauthorized");
    }

    public void ValidateAllowedSender(Guid senderId)
    {
        if (!this.AllowedSenders.Exists(x => x.Id == senderId))
            throw new CourierException("Unauthorized");
    }
}
