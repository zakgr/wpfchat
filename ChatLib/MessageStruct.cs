using System;
using System.Collections.Generic;

namespace ChatLib
{
    public class MessageInfo
    {
        public MessageInfo()
        {
            Type = CommandType.Message;
            UsersRecipient = new List<string>();
        }
        public List<string> UsersRecipient { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
        public CommandType Type { get; set; }
    }

    public enum CommandType
    {
        Users,
        Message,
        Status,
        Room
    }
    
}
