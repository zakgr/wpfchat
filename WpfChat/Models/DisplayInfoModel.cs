﻿using System;
using System.ComponentModel;

namespace WpfChat.Models
{
    public class DisplayInfoModel : INotifyPropertyChanged
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
