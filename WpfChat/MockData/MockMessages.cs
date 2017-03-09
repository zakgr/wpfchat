using System;
using System.Collections.Generic;
using WpfChat.Models;

namespace WpfChat.MockData
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
