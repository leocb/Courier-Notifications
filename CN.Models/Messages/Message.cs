using System;
using System.Collections.Generic;
using System.Text;

namespace CN.Models.Messages
{
    public class Message
    {
        public Guid? Center { get; set; }
        public Guid? Space { get; set; }
        public DateTime? Date { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
