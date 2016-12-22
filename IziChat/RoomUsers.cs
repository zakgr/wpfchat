using System;
using System.Collections.Generic;

namespace IziChat
{
    public class RoomUsers
    {
        public string RoomName { get; set; }
        public Guid RoomId { get; set; }
        public List<string> UserNames { get; set; }
    }
}
