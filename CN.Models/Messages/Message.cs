using System;

namespace CN.Models.Messages;

[Serializable]
public class Message
{
    //public Guid Center { get; set; }
    //public Guid Space { get; set; }
    //public Guid From { get; set; }
    public DateTime Date { get; set; }
    public string Text { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public MessageStatus Status { get; set; } = MessageStatus.None;
}
