using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ServerChat
{
    internal class Program
    {
        private static readonly List<ClientOperator> Clients = new List<ClientOperator>();
        private static void PrintMessage(string msg)
        {
            Console.WriteLine(msg);
        }
        private static void Main(string[] args)
        {
            var listener = new TcpListener(IPAddress.Any, 3000);
            listener.Start();

            while (true)
            {
                var client = listener.AcceptTcpClient();
                var clientOperator = new ClientOperator(client);
                clientOperator.MessageRecieved += ClientOperator_MessageRecieved;
                Clients.Add(clientOperator);
            }
        }

        private static void ClientOperator_MessageRecieved(object sender, string e)
        {
            PrintMessage(e);
            foreach (var client in Clients)
            {
                client.Write(e);
            }
        }
    }
}

