using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        Username = "john",
                        Message = "asdf"
                    },
                    new MainWindow.MessageViewModel()
                    {
                        DateTime = DateTime.Now,
                        Username = "john",
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
