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
using System.Collections.Generic;

namespace IziChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : MetroWindow
    {
        public static RoutedCommand CreateRoomCommand = new RoutedCommand();

        private RoomWindow _room;
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

        public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.Register(
            "MessageSendTo", typeof(MessageSendToModel), typeof(MainWindow), new PropertyMetadata(default(MessageSendToModel)));

        public MessageSendToModel MessageSendTo     
        {
            get { return (MessageSendToModel) GetValue(PropertyTypeProperty); }
            set { SetValue(PropertyTypeProperty, value); }
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
            CreateRoomCommand.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(CreateRoomCommand, CreateRoom_OnClick));
            Settings = !File.Exists("settings.json") ? new ChatSettings() { IpAddress = "127.0.0.1", Username = "default" } : JsonConvert.DeserializeObject<ChatSettings>(File.ReadAllText("settings.json"));
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(Settings));
            _client = new ChatClient(IPAddress.Parse(Settings.IpAddress), 3000, Settings.Username);
            Messages = new ObservableCollection<MessageViewModel>();
            Rooms = new ObservableCollection<RoomViewModel>();
            StatusClient = new StatusConnection();
            MessageSendTo = new MessageSendToModel();
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
            switch (MessageSendTo.Type)
            {
               
                case MessageSendToModel.Types.Room:
                    _client.SendRoomMessage(trimText,MessageSendTo.Id);
                    break;
                case MessageSendToModel.Types.Unicast:
                    _client.SendUnicastMessage(trimText,MessageSendTo.DisplayName);
                    break;
                case MessageSendToModel.Types.Broadcast:
                default:
                    _client.SendBroadcastMessage(trimText);
                    break;
            }
            /*
            if (trimText.StartsWith("/"))
            {
                if (trimText.Contains("/room")) CreateRoom_OnClick(null,null);
            }
            else
            {
               
            }
            */
            txt.Text = "";
        }

        private async void ClientChat_OnLoaded(object sender, RoutedEventArgs e)
        {
            _client.Connecting += _client_Connecting;
            _client.Connected += _client_Connected;
            _client.Disconnected += _client_Disconnected;
            await _client.Connect();
            _client.On<BroadcastMessage>(_client_BroadcastMessageReceived);
            _client.On<RoomMessage>(_client_RoomMessageReceived);
            _client.On<UnicastMessage>(_client_UnicastMessageReceived);
            _client.On<ClientStatusChanged>(_client_ClientStatusChanged);
            _client.On<StatusReport>(_client_StatusReport);
            _client.On<CreateRoom>(_client_AddRoom);
        }

        private void _client_RoomMessageReceived(object sender, RoomMessage e)
        {
            Messages.Add(new MessageViewModel() {
                Username = e.Username, DateTime = e.DateTime,
                Message = e.Message,
                Metadata = e.RoomId.ToString()
            });
        }

        private void _client_UnicastMessageReceived(object sender, UnicastMessage e)
        {
            Messages.Add(new MessageViewModel()
            {
                Username = e.Username, DateTime = e.DateTime,
                Message = e.Message,
                Metadata = e.UserReciever
            });
        }
        private void _client_BroadcastMessageReceived(object sender, BroadcastMessage e)
        {
            //if(Messages.Count > 500) Messages.RemoveAt(0);
            Messages.Add(new MessageViewModel()
            {
                Username = e.Username, DateTime = e.DateTime,
                Message = e.Message,
                Metadata = "Home"
            });
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
            ClientData.Users =
                new ObservableCollection<UserViewModel>(e.Usernames.Select(u => new UserViewModel() { UserName = u }));
        }

        private void _client_ClientStatusChanged(object sender, ClientStatusChanged e)
        {
            if (e.Status == "offline")
            {
                ClientData.Users.Remove(ClientData.Users.Single(user => user.UserName == e.Username));
                foreach (var roomUsers in Rooms.Where(room => room.UserNames.Contains(e.Username)))
                {
                    roomUsers.UserNames.Remove(e.Username);
                }
            }
            else ClientData.Users.Add(new UserViewModel() { UserName = e.Username });
        }

        

        private void _client_Disconnected(object sender, EventArgs e)
        {
            StatusClient.Status = "Disconnected";
            StatusClient.ProgressBarVisiblility = Visibility.Collapsed;
            ClientData.Users.Clear();
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


        private void CreateRoom_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var user in ClientData.Users.Where(users => users.IsSelected))
            {
                user.IsSelected = false;
            }
            _room = new RoomWindow(Main) { Owner = this };
            _room.CreateRoom += _room_CreateRoom;
            _room.Show();
        }

        private void _room_CreateRoom(object sender, List<string> usernames)
        {
            var roomName = sender as string;
            //if (string.IsNullOrEmpty(roomName)| usernames.Count==0) return;
            _client.CreateRoom(roomName, usernames);
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {  
            this.Close();
        }


        private void Room_OnMouseClick(object sender, MouseButtonEventArgs e)
        {
            var s = sender as TreeViewItem;
            MessageSendTo.DisplayName = s.Header.ToString();
            MessageSendTo.Id = new Guid(s.Tag.ToString());
            MessageSendTo.Type= MessageSendToModel.Types.Room;
        }

        private void User_OnClick(object sender, RoutedEventArgs e)
        {
            var s = sender as Button;
            MessageSendTo.DisplayName = s.Content.ToString();
            //MessageSendTo.Id = new Guid("");
            MessageSendTo.Type = MessageSendToModel.Types.Unicast;
        }
    }
}
