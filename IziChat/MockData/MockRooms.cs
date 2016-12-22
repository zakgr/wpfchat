using System;
using System.Collections.Generic;
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
                UserNames = new List<string>()
                {
                    "test",
                    "test2"
                }
            },
                    
        };
    }
}
