﻿using System;
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
using Newtonsoft.Json;

namespace IziChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public StatusConnection StatusClient
        {
            get { return (StatusConnection)GetValue(StatusClientProperty); }
            set { SetValue(StatusClientProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusClienCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusClientProperty =
            DependencyProperty.Register("StatusClient", typeof(StatusConnection), typeof(MainWindow), new PropertyMetadata(null));


        public ObservableCollection<string> OnlineUsers
        {
            get { return (ObservableCollection<string>)GetValue(OnlineUsersProperty); }
            set { SetValue(OnlineUsersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OnlineUsersCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnlineUsersProperty =
            DependencyProperty.Register("OnlineUsers", typeof(ObservableCollection<string>), typeof(MainWindow), new PropertyMetadata(null));


        public ObservableCollection<MessageInfo> Messages
        {
            get { return (ObservableCollection<MessageInfo>)GetValue(MessagesProperty); }
            set { SetValue(MessagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessagesProperty =
            DependencyProperty.Register("Messages", typeof(ObservableCollection<MessageInfo>), typeof(MainWindow), new PropertyMetadata(null));


        private readonly ChatClient _client;

        public MainWindow()
        {
            InitializeComponent();
            ChatSettings settings = null;
            settings = !File.Exists("settings.json") ? new ChatSettings() { IpAddress = "127.0.0.1", Username = "default" } : JsonConvert.DeserializeObject<ChatSettings>(File.ReadAllText("settings.json"));
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
            _client = new ChatClient(IPAddress.Parse(settings.IpAddress), 3000, settings.Username);
            //_client.
            Messages = new ObservableCollection<MessageInfo>();
            OnlineUsers = new ObservableCollection<string>();

            StatusClient = new StatusConnection();
        }

        private void _client_MessageReceived(object sender, MessageInfo e)
        {
            if (e.Type == CommandType.Users)
            {
                var users = JsonConvert.DeserializeObject<List<string>>(e.Message);
                foreach (var user in users)
                {
                    if (!OnlineUsers.Contains(user)) OnlineUsers.Add(user);
                }
            }
            else
            {
                if (e.Type == CommandType.Status && e.Message == "offline")
                {
                    OnlineUsers.Remove(e.UserName);
                }
                Messages.Add(e);
            }
            Scroller.ScrollToBottom();
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return) return;
            var txt = (sender as TextBox);
            if (txt == null) return;
            _client.Write(txt.Text);
            txt.Text = "";
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _client.Connecting += _client_Connecting;
            _client.Connected += _client_Connected;
            _client.Disconnected += _client_Disconnected;
            await _client.Connect();
            _client.MessageReceived += _client_MessageReceived;
        }

        private void _client_Disconnected(object sender, EventArgs e)
        {
            StatusClient.Status = "Disconnected";
            StatusClient.ProgressBarVisiblility =Visibility.Collapsed;
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

       
    }
}
