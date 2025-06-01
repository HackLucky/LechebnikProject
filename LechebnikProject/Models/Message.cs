using System;

namespace LechebnikProject.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string MessageText { get; set; }
        public DateTime SendDate { get; set; }
        public string SenderLogin { get; set; }
    }
}
