using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IziChat.Models
{
    public class RoomViewModel
    {
        public RoomViewModel()
        {
            UserNames = new ObservableCollection<string>();
        }

        public string RoomName { get; set; }
        public Guid RoomId { get; set; }
        public ObservableCollection<string> UserNames { get; set; }
    }
}
