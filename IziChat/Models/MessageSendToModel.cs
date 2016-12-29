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
        private string _displayName;
        private Guid _id;
        private Types _type;

        public string DisplayName {
            get { return _displayName; }
            set
            {
                _displayName = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("DisplayName"));
            }
        }
        public Guid Id {
            get { return _id; }
            set
            {
                _id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Id"));
            }
        }

        public Types Type
        {
            get { return _type; }
            set
            {
                _type = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Type"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public enum Types
        {
            Broadcast,
            Unicast,
            Room
        }
    }
}
