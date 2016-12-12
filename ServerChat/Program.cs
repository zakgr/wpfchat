using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerChat
{
    class Program
    {
        static List<ClientOperator> Clients = new List<ClientOperator>();
        static void PrintMessage(string msg) {
            Console.WriteLine(msg);
        }
        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 3000);
            listener.Start();

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                
                    var clientOperator = new ClientOperator(client);
                    clientOperator.MessageRecieved += ClientOperator_MessageRecieved;
                    Clients.Add(clientOperator);
                
            }
        }

        private static void ClientOperator_MessageRecieved(object sender, string e)
        {
            PrintMessage(e);
            foreach (var client in Clients) {
                client.Write(e);
            }
        }
    }
}

