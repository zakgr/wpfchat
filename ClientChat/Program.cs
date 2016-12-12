using System;
using System.Net.Sockets;


namespace ClientChat
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var client = new TcpClient())
            { 
                var messageSerialization = new MessageSerialization(client);
            }
            Console.ReadKey();
        }
    }
}
