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
                
                var messageSerialization = new MessageSerialization(client);

            }
            Console.ReadKey();
        }
    }
}
