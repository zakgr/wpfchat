using System;
using System.Net.Sockets;


namespace ClientChat
{
    class Program
    {
        static void Main(string[] args)
        {
            using (TcpClient client = new TcpClient())
            {
                client.Connect("localhost", 3000);
                var messageSerialization = new MessageSerialization(client);

            }
            Console.ReadKey();
        }
    }
}
