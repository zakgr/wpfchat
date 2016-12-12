using System;
using System.Collections.Generic;
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

namespace IziChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChatClient _client;

        public MainWindow()
        {
            InitializeComponent();
            _client = new ChatClient(IPAddress.Parse("127.0.0.1"), 3000);
            
        }

        private void _client_MessageReceived(object sender, MessageInfo e)
        {
            ChatTextBlock.Text += e.Message + Environment.NewLine;
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
            await _client.Connect();
            _client.MessageReceived += _client_MessageReceived;
        }
    }
}
