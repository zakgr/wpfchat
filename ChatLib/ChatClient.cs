using System;
using System.Diagnostics;
using System.IO;
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
        public event EventHandler<string> Connected;
        public event EventHandler<string> Disconnected;
        public event EventHandler<string> Connecting;

        public async Task Connect()
        {
            _client = new TcpClient();
            Connecting?.Invoke(this, "Connecting");
            while (!_client.Connected)
                try
                {
                    await _client.ConnectAsync(_address.ToString(), _portno);
                    Connected?.Invoke(this, "Connected");
                    _reader = new StreamReader(_client.GetStream());
                    _writer = new StreamWriter(_client.GetStream()) {AutoFlush = true};
                    StartReading();
                    _writer.WriteLine(JsonConvert.SerializeObject(new MessageInfo
                    {
                        UserName = _username + "@" + _client.Client.LocalEndPoint,
                        Type = CommandType.Status,
                        Message = "online",
                        Date = DateTime.Now
                    }));
                }
                catch (Exception ex)
                {
                    await Task.Delay(5000);
                }
        }

        public void Write(string sendMessage)
        {
            try
            {
                var message = new MessageInfo
                {
                    Date = DateTime.Now,
                    Message = sendMessage,
                    Pid = _pid,
                    UserName = _username + "@" + _client.Client.LocalEndPoint
                };
                var json = JsonConvert.SerializeObject(message);
                _writer.WriteLine(json);
            }
            catch
            {
                Disconnected?.Invoke(this, "Disconnected");
                var t = Connect();
            }
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
                Disconnected?.Invoke(this, "Disconnected");
                await Connect();
            }
        }
    }
}