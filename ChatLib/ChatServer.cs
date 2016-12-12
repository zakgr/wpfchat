using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace ChatLib
{
    public class ChatServer
    {
        private readonly List<ClientOperator> _clients = new List<ClientOperator>();
        public event EventHandler<MessageInfo> MessageReceived;
        public event EventHandler<TcpClient> Connected;
        public static List<string> Users;
        private readonly int _portno;

        public ChatServer(int portno)
        {
            _portno = portno;
        }

        public static void AddUsers(string u)
        {
          Users.Add(u);  
        }
        
        public void Run()
        {
            var listener = new TcpListener(IPAddress.Any, _portno);
            Users= new List<string>();
            listener.Start();

            while (true)
            {
                var client = listener.AcceptTcpClient();
                var clientOperator = new ClientOperator(client);
                Connected?.Invoke(this, client);
                clientOperator.Disconnected += ClientOperator_Disconnected;
                clientOperator.MessageRecieved += ClientOperator_MessageRecieved;
                
                _clients.Add(clientOperator);
                
            }
        }

        private void ClientOperator_Disconnected(object sender, TcpClient e)
        {
            Broadcast(new MessageInfo()
            {
                Type = CommandType.Status,
                Message = "offline",
                Date = DateTime.Now,
                UserName = "@"+ e.Client.RemoteEndPoint.ToString()
            });
        }

        private void Broadcast(MessageInfo message)
        {
            foreach (var client in _clients)
            {
                if (message.Type == CommandType.Status)
                {
                    if (message.Message == "online")
                    {
                        message.UserName = "";
                        foreach (var user in Users)
                        {
                            message.UserName += user+",";
                        }
                    }
                }
                client.Write(JsonConvert.SerializeObject(message));
            }
        }

        private void ClientOperator_MessageRecieved(object sender, string e)
        {
            var msgInfo = JsonConvert.DeserializeObject<MessageInfo>(e);
            MessageReceived?.Invoke(this, msgInfo);
            Broadcast(msgInfo);
        }
    }
}
