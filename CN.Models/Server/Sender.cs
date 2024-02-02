
using System;

namespace CN.Models.Server;

public class AllowedSender
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";

    public AllowedSender() { }

    public AllowedSender(Guid Id, string Name)
    {
        this.Id = Id;
        this.Name = Name;
    }
}