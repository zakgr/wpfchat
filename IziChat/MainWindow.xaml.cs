using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChatLib;
using System.IO;
using ChatLib.Models;
using Newtonsoft.Json;

namespace IziChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class MessageViewModel
        {
            public DateTime DateTime { get; set; }

            public string Username { get; set; }

            public string Message { get; set; }
        }

        public StatusConnection StatusClient
        {
            get { return (StatusConnection)GetValue(StatusClientProperty); }
            set { SetValue(StatusClientProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusClienCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusClientProperty =
            DependencyProperty.Register("StatusClient", typeof(StatusConnection), typeof(MainWindow), new PropertyMetadata(null));


        public ObservableCollection<UserSelection> OnlineUsers
        {
            get { return (ObservableCollection<UserSelection>)GetValue(OnlineUsersProperty); }
            set { SetValue(OnlineUsersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OnlineUsersCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnlineUsersProperty =
            DependencyProperty.Register("OnlineUsers", typeof(ObservableCollection<UserSelection>), typeof(MainWindow), new PropertyMetadata(null));


        public ObservableCollection<MessageViewModel> Messages
        {
            get { return (ObservableCollection<MessageViewModel>)GetValue(MessagesProperty); }
            set { SetValue(MessagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessagesProperty =
            DependencyProperty.Register("Messages", typeof(ObservableCollection<MessageViewModel>), typeof(MainWindow), new PropertyMetadata(null));


        private readonly ChatClient _client;

        public MainWindow()
        {
            InitializeComponent();
            ChatSettings settings = null;
            settings = !File.Exists("settings.json") ? new ChatSettings() { IpAddress = "127.0.0.1", Username = "default" } : JsonConvert.DeserializeObject<ChatSettings>(File.ReadAllText("settings.json"));
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
            _client = new ChatClient(IPAddress.Parse(settings.IpAddress), 3000, settings.Username);
            //_client.
            Messages = new ObservableCollection<MessageViewModel>();
            OnlineUsers = new ObservableCollection<UserSelection>();
            
            StatusClient = new StatusConnection();
        }

        private void _client_MessageReceived(object sender, Command<BroadcastMessage> e)
        {
        //    switch (e.Type)
        //    {
        //        case CommandType.Users:
        //        {
        //            var users = JsonConvert.DeserializeObject<List<string>>(e.Message);
        //            List<string> localusers = new List<string>();
        //            foreach (var onlineUser in OnlineUsers)
        //            {
        //                localusers.Add(onlineUser.UserName);
        //            }
        //            foreach (var user in users)
        //            {
        //                UserSelection userSelection = new UserSelection() { UserName = user, IsSelected = false };
        //                if (!localusers.Contains(user)) OnlineUsers.Add(userSelection);
        //            }
        //        }
        //            break;
        //        case CommandType.Room:
        //        {
        //            var users = e.UsersRecipient;
        //        }
        //            break;
               
        //    }
            Scroller.ScrollToBottom();
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return) return;
            var txt = (sender as TextBox);

            if (txt == null || txt?.Text.Trim() == "") return;
            if (txt.Text.Trim().StartsWith("/"))
            {
                CreateRoom(txt.Text.Trim());

            }
            else
            {
                _client.SendBroadcastMessage(txt.Text.Trim());
            }
            txt.Text = "";
        }

        private async void ClientChat_OnLoaded(object sender, RoutedEventArgs e)
        {
            _client.Connecting += _client_Connecting;
            _client.Connected += _client_Connected;
            _client.Disconnected += _client_Disconnected;
            await _client.Connect();
            _client.BroadcastMessageReceived += _client_BroadcastMessageReceived;
            _client.ClientStatusChanged += _client_ClientStatusChanged;
            _client.StatusReport += _client_StatusReport;
        }

        private void _client_StatusReport(object sender, Command<StatusReport> e)
        {
            OnlineUsers =
                new ObservableCollection<UserSelection>(e.Data.Usernames.Select(u => new UserSelection() {UserName = u}));
        }

        private void _client_ClientStatusChanged(object sender, Command<ClientStatusChangedData> e)
        {
            if (e.Data.Status == "offline")
            {
                OnlineUsers.Remove(OnlineUsers.Single(user => user.UserName == e.Data.Username));
            }
            OnlineUsers.Add(new UserSelection() {UserName = e.Data.Username});
        }

        private void _client_BroadcastMessageReceived(object sender, Command<BroadcastMessage> e)
        {
            Messages.Add(new MessageViewModel() { Username = e.Data.Username, DateTime = e.DateTime, Message = e.Data.Message});
        }

        private void _client_Disconnected(object sender, EventArgs e)
        {
            StatusClient.Status = "Disconnected";
            StatusClient.ProgressBarVisiblility = Visibility.Collapsed;
            OnlineUsers.Clear();
        }

        private void _client_Connected(object sender, EventArgs e)
        {
            StatusClient.Status = "Connected";
            StatusClient.ProgressBarVisiblility = Visibility.Collapsed;
        }

        private void _client_Connecting(object sender, EventArgs e)
        {
            StatusClient.Status = "Connecting";
            StatusClient.ProgressBarVisiblility = Visibility.Visible;
        }

        private void CreateRoom(string text)
        {
           _client.CreateRoom(text,OnlineUsers.Where(user => user.IsSelected).Select(user=>user.UserName).ToList());
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var txt = (sender as TextBox) ?? new TextBox() {Text = ""};
            CreateRoom(txt.Text);
        }
    }
}
