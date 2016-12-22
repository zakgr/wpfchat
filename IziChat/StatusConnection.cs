using System.ComponentModel;
using System.Windows;

namespace IziChat
{
    public class StatusConnection :INotifyPropertyChanged
    {
        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
            }
        }

        private Visibility _progressVisibility;

        public Visibility ProgressBarVisiblility
        {
            get { return _progressVisibility; }
            set
            {
                _progressVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProgressBarVisiblility"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
