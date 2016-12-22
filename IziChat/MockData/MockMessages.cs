using System;
using System.Collections.Generic;
using IziChat.Models;

namespace IziChat.MockData
{
    internal class MockMessages
    {
        public List<MessageViewModel> Messages => new List<MessageViewModel>()
        {
            new MessageViewModel()
            {
                DateTime = DateTime.Now,
                Username = "jos",
                Message = "asdf"
            },
            new MessageViewModel()
            {
                DateTime = DateTime.Now,
                Username = "default",
                Message = "asdf"
            }
            ,
            new MessageViewModel()
            {
                DateTime = DateTime.Now,
                Username = "john",
                Message = "asdf"
            }
        };
    }
}
