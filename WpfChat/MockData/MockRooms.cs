﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WpfChat.Models;

namespace WpfChat.MockData
{
    class MockRooms
    {
        public List<RoomViewModel> Rooms => new List<RoomViewModel>()
        {
            new RoomViewModel()
            {
                RoomId = Guid.NewGuid(),
                RoomName = "Default",
                UserNames = new ObservableCollection<string>()
                {
                    "test",
                    "test2"
                }
            },
                    
        };
    }
}
