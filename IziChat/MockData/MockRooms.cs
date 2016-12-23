using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using IziChat.Models;

namespace IziChat.MockData
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
