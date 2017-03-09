using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfChat.Models;

namespace WpfChat
{
    public static class ClientData
    {
        
        private static ObservableCollection<UserViewModel> _users = new ObservableCollection<UserViewModel>();

        public static ObservableCollection<UserViewModel> Users
        {
            get { return _users; }

            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        private static void OnPropertyChanged([CallerMemberName] string name=null)
        {
            StaticPropertyChanged?.Invoke(null,new PropertyChangedEventArgs(name));
        }

        public static event PropertyChangedEventHandler StaticPropertyChanged;
    }
}
