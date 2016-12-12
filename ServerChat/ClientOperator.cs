using System;
using System.IO;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Threading;

namespace ServerChat
{
    internal class ClientOperator
    {
        private readonly StreamReader _reader;
        private static StreamWriter _writer;
        public ClientOperator(TcpClient client)
        {
            _reader = new StreamReader(client.GetStream());
            _writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
            StartReading();
        }
        private void StartReading()
        {
            var thread = new Thread(() =>
            {
                try
                {
                    string recievedMessage = null;
                    while ((recievedMessage = _reader.ReadLine()) != null)
                    {
                        MessageRecieved?.Invoke(this, recievedMessage);
                    }
                }
                catch {Console.WriteLine("No Client Connected"); }
            });
            thread.Start();
        }
        public void Write(string message)
        {
            _writer.WriteLine(message);
        }
        public event EventHandler<string> MessageRecieved;
    }
}
