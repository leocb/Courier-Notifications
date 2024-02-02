using System;

namespace CN.Models.Messages;

[Serializable]
public class Message
{
    public Guid From { get; set; }
    public MessageStatus Status { get; set; } = MessageStatus.None;
    public DateTime Date { get; set; }
    public string Text { get; set; } = string.Empty;
}
