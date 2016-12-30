using System;

namespace IziChat.Models
{
    public class MessageViewModel
    {
        public DateTime DateTime { get; set; }

        public string Username { get; set; }

        public string Message { get; set; }
        public string TargetInfo { get; set; }
    }
}