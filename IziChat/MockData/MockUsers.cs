using System.Collections.Generic;
using System.Collections.ObjectModel;
using IziChat.Models;

namespace IziChat.MockData
{
    class MockUsers
    {
        public static List<UserViewModel> Users => new List<UserViewModel>()
        {
            new UserViewModel()
            {
                IsSelected = false,
                UserName = "test"
            },
            new UserViewModel()
            {
                IsSelected = false,
                UserName = "test2"
            }

        };
    }
}