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
    public class ChatServer : CommandHandler
    {
        //public event EventHandler<MessageInfo> MessageReceived;
        public event EventHandler<TcpClient> Connected;
        public Dictionary<string, ClientOperator> Users { get; private set; }
        private readonly int _portno;

        public ChatServer(int portno)
        {
            _portno = portno;
            On<ClientStatusChanged>(OnStatusChanged);
            On<BroadcastMessage>(OnBroadcastMessage);
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
                Task.Run(async () =>
                {
                    await Task.Delay(500);
                    for (int i = 0; i < 100000; i++)
                    {
                        Broadcast(new BroadcastMessage()
                        {
                            Message = "sss" + i.ToString(),
                            Username = "ffff"
                        });
                    }
                });
                
            }
        }

        private void ClientOperator_Disconnected(object sender, TcpClient e)
        {
            var username = Users.FindByClient(e);
            lock (Users)
            {
               Users.Remove(username);
            }
            Broadcast(new ClientStatusChanged()
            {
                Status = "offline",
                Username = username
            });
        }

        
        private void Broadcast(BaseCommand message)
        {
            lock (Users)
            {
                foreach (var client in Users.Select(kv => kv.Value))
                {
                    client.Write(
                        JsonConvert.SerializeObject(new CommandWrapper() {Type = message.GetType(), Overhead = message}));
                }
            }

        }

        private void UniCast(BaseCommand message, ICollection<string> users)
        {
            foreach (var client in Users.Where(kv => users.Contains(kv.Key)).Select(kv => kv.Value))
            {
                client.Write(JsonConvert.SerializeObject(new CommandWrapper() { Type = message.GetType(), Overhead = message }));
            }
        }

        private void ClientOperator_DataReceived(object sender, string e)
        {
            var generalCommand = JsonConvert.DeserializeObject<CommandWrapper>(e);
            var command = JsonConvert.DeserializeObject(generalCommand.Overhead.ToString(), generalCommand.Type);
            Invoke(sender, command as BaseCommand);

        }
        /*
        private void InspectMessage(ClientOperator tcpClient, BaseCommand)
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
            
        }

       */

        private void OnBroadcastMessage(object sender, BroadcastMessage e)
        {
            e.Username = Users.FindByClient((sender as ClientOperator).TcpClient);
            Broadcast(e);
        }

        private void OnStatusChanged(object client, ClientStatusChanged e)
        {
            var oper = client as ClientOperator;
            if (oper == null) return;
            Broadcast(new ClientStatusChanged()
            {
                Status = e.Status,
                Username = e.Username + "@" + (oper.TcpClient.Client.RemoteEndPoint as IPEndPoint)
            });
            if (e.Status == "online")
            {
                var username = e.Username + "@" + (oper.TcpClient.Client.RemoteEndPoint as IPEndPoint);
                Users.Add(username, oper);

                UniCast(new StatusReport()
                {
                    Usernames = Users.Keys.ToList()
                }, new List<string>() {username});
            }
        }

    }
}
