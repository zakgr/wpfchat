using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ChatLib.Models;
using Newtonsoft.Json;

namespace ChatLib
{
    public class ChatServer
    {
        //public event EventHandler<MessageInfo> MessageReceived;
        public event EventHandler<TcpClient> Connected;
        public Dictionary<string, ClientOperator> Users { get; private set; }
        private readonly int _portno;

        public ChatServer(int portno)
        {
            _portno = portno;
        }

        public void Run()
        {
            var listener = new TcpListener(IPAddress.Any, _portno);
            Users = new Dictionary<string, ClientOperator>();
            listener.Start();

            while (true)
            {
                var client = listener.AcceptTcpClient();
                var clientOperator = new ClientOperator(client);
                Connected?.Invoke(this, client);
                
                clientOperator.Disconnected += ClientOperator_Disconnected;
                clientOperator.DataReceived += ClientOperator_DataReceived;
                clientOperator.StartReading();
            }
        }

        private void ClientOperator_Disconnected(object sender, TcpClient e)
        {
            var username = Users.FindByClient(e);
            Users.Remove(username);

            var statusReport = Command.CreateCommand(
                CommandType.StatusChange,
                new ClientStatusChangedData()
                {
                    Status = "offline",
                    Username = username
                });
            Broadcast(statusReport);
        }

        private void Broadcast<T>(Command<T> message)
        {
            foreach (var client in Users.Select(kv=>kv.Value))
            {
                client.Write(JsonConvert.SerializeObject(message));
            }
        }

        private void UniCast<T>(Command<T> message, List<string> users )
        {
            foreach (var client in Users.Where(kv => users.Contains(kv.Key)).Select(kv => kv.Value))
            {
                client.Write(JsonConvert.SerializeObject(message));
            }
        }

        private void ClientOperator_DataReceived(object sender, string e)
        {
            var command = JsonConvert.DeserializeObject<Command<object>>(e);
            
            InspectMessage(sender as ClientOperator, command.CommandType, e);
        }

        private void InspectMessage(ClientOperator tcpClient, CommandType type, string json)
        {
            switch (type)
            {
                case CommandType.StatusChange:
                    StatusType(tcpClient,JsonConvert.DeserializeObject<Command<StatusData>>(json));
                    break;
                case CommandType.CreateRoom:
                    //RoomType(msgInfo);
                    break;
                case CommandType.BroadcastMessage:
                    var msg = JsonConvert.DeserializeObject<Command<BroadcastMessage>>(json);
                    msg.Data.Username = Users.FindByClient(tcpClient.TcpClient);
                    Broadcast(msg);
                    break;

                case CommandType.RoomMessage:
                    //RoomType(msgInfo);
                    break;
            }
        }

        private void RoomType(Command<CreateRoom> msgInfo)
        {
            /*
            var roomMessage = new MessageInfo()
            {
                Type = CommandType.Room,
                Message = msgInfo.UserName + " invite you to his Room",
                Date = DateTime.Now,
                UserName = msgInfo.UserName,
                UsersRecipient = msgInfo.UsersRecipient
            };
            MessageReceived?.Invoke(this, roomMessage);
            UniCast(roomMessage);
            */
        }

       

        private void StatusType(ClientOperator tcpClient, Command<StatusData> msgInfo)
        {
            //MessageReceived?.Invoke(this, msgInfo);
            var command = Command.CreateCommand(CommandType.StatusChange, new ClientStatusChangedData()
            {
                Status = msgInfo.Data.Status,
                Username = msgInfo.Data.Username + "@" + (tcpClient.TcpClient.Client.RemoteEndPoint as IPEndPoint)
            });
            Broadcast(command);
            if (msgInfo.Data.Status == "online")
            {
                var username = msgInfo.Data.Username + "@" + (tcpClient.TcpClient.Client.RemoteEndPoint as IPEndPoint);
                Users.Add(username, tcpClient);
                var statusReport = Command.CreateCommand<StatusReport>(CommandType.StatusReport, new StatusReport()
                {
                    Usernames = Users.Keys.ToList()
                });
                UniCast(statusReport, new List<string>() { username });
            }
        }
    }
}
