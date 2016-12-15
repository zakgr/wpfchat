using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using ChatLib;

namespace ServerChat
{
    public static class Program
    {
        private static ChatServer _server;
        static void Main()
        {
            _server = new ChatServer(3000);
            //_server.MessageReceived += _server_MessageReceived;
            _server.Run();
        }

        //private static void _server_MessageReceived(object sender, MessageInfo e)
        //{
        //    Console.WriteLine($"roufianos edw o {e.UserName} eipe {e.Message}");
        //}
    }
}

