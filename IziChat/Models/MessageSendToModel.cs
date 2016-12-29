using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IziChat.Models
{
    public class MessageSendToModel : INotifyPropertyChanged
    {
        private string _displayName, _id;
        public string DisplayName {
            get { return _displayName; }
            set
            {
                _displayName = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("DisplayName"));
            }
        }
        public string Id {
            get { return _id; }
            set
            {
                _id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Id"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
