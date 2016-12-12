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

        private readonly int _portno;

        public ChatServer(int portno)
        {
            _portno = portno;
        }

        
        public void Run()
        {
            var listener = new TcpListener(IPAddress.Any, _portno);
            listener.Start();

            while (true)
            {
                var client = listener.AcceptTcpClient();
                var clientOperator = new ClientOperator(client);
                Connected?.Invoke(this, client);
                Broadcast(new MessageInfo()
                {
                    Type = CommandType.Status,
                    Message = "online",
                    UserName = client.Client.RemoteEndPoint.ToString()
                });
                clientOperator.MessageRecieved += ClientOperator_MessageRecieved;
                
                _clients.Add(clientOperator);
            }
        }

        private void Broadcast(MessageInfo message)
        {
            foreach (var client in _clients)
            {
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
