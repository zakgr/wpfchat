using System;
using System.Collections.Generic;

namespace IziChat
{
    class MockMessages
    {
        public List<MainWindow.MessageViewModel> Messages
        {
            get
            {
                return new List<MainWindow.MessageViewModel>()
                {
                    new MainWindow.MessageViewModel()
                    {
                        DateTime = DateTime.Now,
                        Username = "jos",
                        Message = "asdf"
                    },
                    new MainWindow.MessageViewModel()
                    {
                        DateTime = DateTime.Now,
                        Username = "default",
                        Message = "asdf"
                    }
                    ,
                    new MainWindow.MessageViewModel()
                    {
                        DateTime = DateTime.Now,
                        Username = "john",
                        Message = "asdf"
                    }
                };
            } 
        }
    }
}
