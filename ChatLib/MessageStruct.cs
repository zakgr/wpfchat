using System;

namespace ChatLib
{
    public class MessageInfo
    {
        public MessageInfo()
        {
            Type = CommandType.Message;
        }

        public string Message { get; set; }
        public int Pid { get; set; }

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
