namespace WpfChat.Models
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            IsSelected = false;
        }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
    }
}
