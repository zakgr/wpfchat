using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChatLib
{
    public class ChatServer
    {
        private readonly List<ClientOperator> _clients = new List<ClientOperator>();
        public event EventHandler<MessageInfo> MessageReceived;
        public event EventHandler<TcpClient> Connected;
        public Dictionary<string, TcpClient> Users { get; private set; }
        private readonly int _portno;

        public ChatServer(int portno)
        {
            _portno = portno;
        }

        public void Run()
        {
            var listener = new TcpListener(IPAddress.Any, _portno);
            Users = new Dictionary<string, TcpClient>();
            listener.Start();

            while (true)
            {
                var client = listener.AcceptTcpClient();
                var clientOperator = new ClientOperator(client);
                Connected?.Invoke(this, client);
                _clients.Add(clientOperator);
                clientOperator.Disconnected += ClientOperator_Disconnected;
                clientOperator.MessageRecieved += ClientOperator_MessageRecieved;
                clientOperator.StartReading();
            }
        }

        private void ClientOperator_Disconnected(object sender, TcpClient e)
        {
            var username = Users.FindByClient(e);
            Users.Remove(username);
            Broadcast(new MessageInfo()
            {
                Type = CommandType.Status,
                Message = "offline",
                Date = DateTime.Now,
                UserName = username
            });
        }

        private void Broadcast(MessageInfo message)
        {
            foreach (var client in _clients)
            {
                client.Write(JsonConvert.SerializeObject(message));
            }
        }
        private void UniCast(MessageInfo message)
        {
            foreach (var client in _clients)
            {
                var clientname = client.TcpClient.Client.RemoteEndPoint.ToString();
                if (message.UsersRecipient.Any(user => user.Contains(clientname)))
                    client.Write(JsonConvert.SerializeObject(message));
            }
        }
        private void ClientOperator_MessageRecieved(object sender, string e)
        {
            var msgInfo = JsonConvert.DeserializeObject<MessageInfo>(e);
            InspectMessage((sender as ClientOperator).TcpClient, msgInfo);
            MessageReceived?.Invoke(this, msgInfo);
            if (msgInfo.UsersRecipient == null) Broadcast(msgInfo);
            else if (msgInfo.UsersRecipient.Any(user => user.Contains("All"))) Broadcast(msgInfo);
            else UniCast(msgInfo);
        }

        private void InspectMessage(TcpClient tcpClient, MessageInfo msgInfo)
        {
            if (msgInfo.Type == CommandType.Status)
            {
                StatusType(tcpClient, msgInfo);
            }
            else if (msgInfo.Type == CommandType.Room)
            {
                RoomType(msgInfo);
            }
        }

        private void RoomType(MessageInfo msgInfo)
        {
            var roomMessage = new MessageInfo()
            {
                Type = CommandType.Room,
                Message = msgInfo.UserName + " invite you to his Room",
                Date = DateTime.Now,
                UserName = msgInfo.UserName,
                UsersRecipient = msgInfo.UsersRecipient
            };

            foreach (var client in _clients)
            {
                var clientname = client.TcpClient.Client.RemoteEndPoint.ToString();
                if (msgInfo.UsersRecipient.Any(user => user.Contains(clientname)))
                    client.Write(JsonConvert.SerializeObject(roomMessage));
            }
        }

        private void StatusType(TcpClient tcpClient, MessageInfo msgInfo)
        {
            if (msgInfo.Message == "online")
            {
                Users.Add(msgInfo.UserName + "@" + (tcpClient.Client.RemoteEndPoint as IPEndPoint), tcpClient);
                var loginMessage = new MessageInfo()
                {
                    Type = CommandType.Users,
                    Message = JsonConvert.SerializeObject(Users.Keys),
                    Date = DateTime.Now,
                    UserName = "Users"
                };
                foreach (var client in _clients)
                {
                    client.Write(JsonConvert.SerializeObject(loginMessage));
                }
            }
        }
    }
}
