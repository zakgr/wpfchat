using System;

namespace ChatLib
{
    public class MessageInfo
    {
        public MessageInfo()
        {
            Type = CommandType.Message;
        }

        public string Message;
        public int Pid;
        public string UserName;
        public DateTime Date;
        public CommandType Type;
    }

    public enum CommandType
    {
        Message,
        Status,
        Room
    }
    
}
