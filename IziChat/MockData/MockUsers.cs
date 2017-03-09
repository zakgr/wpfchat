using System.Collections.Generic;
using WpfChat.Models;

namespace WpfChat.MockData
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