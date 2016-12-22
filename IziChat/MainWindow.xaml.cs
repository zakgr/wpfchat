using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChatLib;
using System.IO;
using ChatLib.Models;
using IziChat.Models;
using MahApps.Metro.Controls;
using Newtonsoft.Json;

namespace IziChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {


        public StatusConnection StatusClient
        {
            get { return (StatusConnection)GetValue(StatusClientProperty); }
            set { SetValue(StatusClientProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusClienCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusClientProperty =
            DependencyProperty.Register("StatusClient", typeof(StatusConnection), typeof(MainWindow), new PropertyMetadata(null));



        public ObservableCollection<RoomViewModel> Rooms
        {
            get { return (ObservableCollection<RoomViewModel>)GetValue(RoomsProperty); }
            set { SetValue(RoomsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RoomLists.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RoomsProperty =
            DependencyProperty.Register("Rooms", typeof(ObservableCollection<RoomViewModel>), typeof(MainWindow), new PropertyMetadata(null));



        public ObservableCollection<UserViewModel> OnlineUsers
        {
            get { return (ObservableCollection<UserViewModel>)GetValue(OnlineUsersProperty); }
            set { SetValue(OnlineUsersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OnlineUsersCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnlineUsersProperty =
            DependencyProperty.Register("OnlineUsers", typeof(ObservableCollection<UserViewModel>), typeof(MainWindow), new PropertyMetadata(null));


        public ObservableCollection<MessageViewModel> Messages
        {
            get { return (ObservableCollection<MessageViewModel>)GetValue(MessagesProperty); }
            set { SetValue(MessagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessagesProperty =
            DependencyProperty.Register("Messages", typeof(ObservableCollection<MessageViewModel>), typeof(MainWindow), new PropertyMetadata(null));


        private readonly ChatClient _client;

        public static readonly DependencyProperty IpProperty = DependencyProperty.Register(
            "Ip", typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));

        public string Ip
        {
            get { return (string)GetValue(IpProperty); }
            set { SetValue(IpProperty, value); }
        }
        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register(
            "Settings", typeof(ChatSettings), typeof(MainWindow), new PropertyMetadata(default(ChatSettings)));

        public ChatSettings Settings
        {
            get { return (ChatSettings)GetValue(SettingsProperty); }
            set { SetValue(SettingsProperty, value); }
        }
        public MainWindow()
        {

            InitializeComponent();
            Settings = !File.Exists("settings.json") ? new ChatSettings() { IpAddress = "127.0.0.1", Username = "default" } : JsonConvert.DeserializeObject<ChatSettings>(File.ReadAllText("settings.json"));
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(Settings));
            _client = new ChatClient(IPAddress.Parse(Settings.IpAddress), 3000, Settings.Username);
            Messages = new ObservableCollection<MessageViewModel>();
            OnlineUsers = new ObservableCollection<UserViewModel>();
            Rooms = new ObservableCollection<RoomViewModel>();
            StatusClient = new StatusConnection();
        }

        //private void _client_MessageReceived(object sender, Command<BroadcastMessage> e)
        //{
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
        //   Scroller.ScrollToBottom();
        // }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return) return;
            var txt = (sender as TextBox);

            if (txt == null || txt.Text.Trim() == "") return;
            var trimText = txt.Text.Trim();
            if (trimText.StartsWith("/"))
            {
                if (trimText.Contains("/room")) CreateRoom(trimText);
            }
            else
            {
                _client.SendBroadcastMessage(trimText);
            }
            txt.Text = "";
        }

        private async void ClientChat_OnLoaded(object sender, RoutedEventArgs e)
        {
            _client.Connecting += _client_Connecting;
            _client.Connected += _client_Connected;
            _client.Disconnected += _client_Disconnected;
            await _client.Connect();
            _client.On<BroadcastMessage>(_client_BroadcastMessageReceived);
            _client.On<ClientStatusChanged>(_client_ClientStatusChanged);
            _client.On<StatusReport>(_client_StatusReport);
            _client.On<CreateRoom>(_client_AddRoom);
        }

        private void CreateRoom(string text)
        {
            if (string.IsNullOrEmpty(text)) text = "default";
            _client.CreateRoom(text, OnlineUsers.Where(user => user.IsSelected).Select(user => user.UserName).ToList());
        }

        private void _client_AddRoom(object sender, CreateRoom e)
        {
            Rooms.Add(new RoomViewModel()
            {
                RoomId = e.RoomId,
                RoomName = e.RoomName,
                UserNames = new ObservableCollection<string>(e.Users)
            });
        }

        private void _client_StatusReport(object sender, StatusReport e)
        {
            OnlineUsers =
                new ObservableCollection<UserViewModel>(e.Usernames.Select(u => new UserViewModel() { UserName = u }));
        }

        private void _client_ClientStatusChanged(object sender, ClientStatusChanged e)
        {
            if (e.Status == "offline")
            {
                OnlineUsers.Remove(OnlineUsers.Single(user => user.UserName == e.Username));
                foreach (var roomUsers in Rooms.Where(room => room.UserNames.Contains(e.Username)))
                {
                    roomUsers.UserNames.Remove(e.Username);
                }
            }
            else OnlineUsers.Add(new UserViewModel() { UserName = e.Username });
        }

        private void _client_BroadcastMessageReceived(object sender, BroadcastMessage e)
        {
            //if(Messages.Count > 500) Messages.RemoveAt(0);
            Messages.Add(new MessageViewModel() { Username = e.Username, DateTime = e.DateTime, Message = e.Message });
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
            Ip = "@" + (_client.TcpClient.Client.LocalEndPoint as IPEndPoint);
        }

        private void _client_Connecting(object sender, EventArgs e)
        {
            StatusClient.Status = "Connecting";
            StatusClient.ProgressBarVisiblility = Visibility.Visible;
        }



        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var txt = (sender as TextBox) ?? new TextBox() { Text = "" };
            CreateRoom(txt.Text);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            new Window().Show();
        }
    }
}
