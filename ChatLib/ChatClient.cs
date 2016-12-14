using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChatLib
{
    public class ChatClient
    {
        private readonly IPAddress _address;
        private readonly int _pid;
        private readonly int _portno;
        private readonly string _username;
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;
        public string Status;

        public ChatClient(IPAddress address, int portno, string username)
        {
            _address = address;
            _portno = portno;
            _username = username;
            _pid = Process.GetCurrentProcess().Id;
        }

        public event EventHandler<MessageInfo> MessageReceived;
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler Connecting;

        public async Task Connect()
        {
            _client = new TcpClient();
            Connecting?.Invoke(this, EventArgs.Empty);
            while (!_client.Connected)
                try
                {
                    await _client.ConnectAsync(_address.ToString(), _portno);
                    Connected?.Invoke(this, EventArgs.Empty);
                    _reader = new StreamReader(_client.GetStream());
                    _writer = new StreamWriter(_client.GetStream()) { AutoFlush = true };
                    StartReading();
                    _writer.WriteLine(JsonConvert.SerializeObject(new MessageInfo
                    {
                        UserName = _username,
                        Type = CommandType.Status,
                        Message = "online",
                        Date = DateTime.Now
                    }));
                }
                catch (Exception)
                {
                    await Task.Delay(5000);
                }
        }

        private void Write(MessageInfo msg)
        {
            try
            {
                var json = JsonConvert.SerializeObject(msg);
                _writer.WriteLine(json);
            }
            catch (Exception)
            {
                Disconnected?.Invoke(this, EventArgs.Empty);
                var t = Connect();
            }
        }

        public void CreateRoom(string sendMessage,List<string> usernames)
        {

            var message = new MessageInfo
            {
                Date = DateTime.Now,
                Message = "",
                Type = CommandType.Room,
                UserName = _username,
                UsersRecipient = usernames
            };
            Write(message);

        }

        public void SendMessage(string sendMessage, List<string> usernames)
        {
            if (usernames == null | usernames?.Count==0)
            {
                usernames = new List<string>();
                usernames.Add("All");
            }
            else
            {
                var localuser = _username + "@" + _client.Client.LocalEndPoint.ToString();
                if (!usernames.Any(user=>user.Contains(localuser))) usernames.Add(localuser);
            }            
            var message = new MessageInfo
            {
                Date = DateTime.Now,
                Message = sendMessage,
                Type = CommandType.Message,
                UserName = _username,
                UsersRecipient = usernames
            };
            Write(message);
        }


        private async void StartReading()
        {
            try
            {
                string recievedMessage = null;
                Console.WriteLine("Listening");
                while ((recievedMessage = await _reader.ReadLineAsync()) != null)
                {
                    var input = JsonConvert.DeserializeObject<MessageInfo>(recievedMessage);
                    MessageReceived?.Invoke(this, input);
                }
            }
            catch
            {
                Disconnected?.Invoke(this, EventArgs.Empty);
                await Connect();
            }
        }
    }
}