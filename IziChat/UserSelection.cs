using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IziChat
{
    public class UserSelection
    {
        public UserSelection()
        {
            IsSelected = false;
        }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
    }
}
