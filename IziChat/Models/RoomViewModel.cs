using System;
using System.Collections.Generic;

namespace IziChat.Models
{
    public class RoomViewModel
    {
        public RoomViewModel()
        {
            UserNames = new List<string>();
        }

        public string RoomName { get; set; }
        public Guid RoomId { get; set; }
        public List<string> UserNames { get; set; }
    }
}
