using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChatLib
{
    public class ChatClient
    {
        private StreamReader _reader;
        private StreamWriter _writer;
        private readonly int _pid;
        private readonly int _portno;
        private readonly IPAddress _address;
        private TcpClient _client;

        public event EventHandler<MessageInfo> MessageReceived;
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler Connecting;
        private readonly string _username;

        public ChatClient(IPAddress address, int portno, string username)
        {
            _address = address;
            _portno = portno;
            _username = username;
            _pid = Process.GetCurrentProcess().Id;
        }

        public async Task Connect()
        {
            _client = new TcpClient();
            Connecting?.Invoke(this, EventArgs.Empty);
            while (!_client.Connected)
            {
                try
                {
                    await _client.ConnectAsync(_address.ToString(), _portno);
                    Connected?.Invoke(this, EventArgs.Empty);
                    _reader = new StreamReader(_client.GetStream());
                    _writer = new StreamWriter(_client.GetStream()) { AutoFlush = true };
                    _writer.WriteLine(JsonConvert.SerializeObject(new MessageInfo()
                    {
                        UserName = _username+ "@"+ _client.Client.LocalEndPoint.ToString(),
                        Type = CommandType.Status,
                        Message = "online",
                        Date = DateTime.Now
                    }));
                    StartReading();
                }
                catch(Exception ex)
                {
                    await Task.Delay(5000);
                }
            }

           
            
        }

        public void Write(string sendMessage)
        {
            try
            {
                var message = new MessageInfo()
                {
                    Date = DateTime.Now,
                    Message = sendMessage,
                    Pid = _pid,
                    UserName = _username + "@" + _client.Client.LocalEndPoint.ToString()
                };
                var json = JsonConvert.SerializeObject(message);
                _writer.WriteLine(json);
            }
            catch
            {
                Disconnected?.Invoke(this, EventArgs.Empty);
                var t = Connect();
            }
        }


        private async void StartReading()
        {
            try
            {
                string recievedMessage = null;
                while ((recievedMessage = await _reader.ReadLineAsync()) != null)
                {
                    var input = JsonConvert.DeserializeObject<MessageInfo>(recievedMessage);
                    MessageReceived?.Invoke(this, input);
                }
            }
            catch
            {
                Disconnected?.Invoke(this, EventArgs.Empty);
                Connect();
            }
        }
    }
}
