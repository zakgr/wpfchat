using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace ChatLib
{
    internal class ClientOperator
    {
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;
        private readonly TcpClient _client;
        public event EventHandler<TcpClient> Disconnected;

        public ClientOperator(TcpClient client)
        {
            _client = client;
            _reader = new StreamReader(client.GetStream());
            _writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
            StartReading();
        }

        private async void StartReading()
        {
            try
            {
                string recievedMessage = null;
                while ((recievedMessage = await _reader.ReadLineAsync()) != null)
                {
                    MessageRecieved?.Invoke(this, recievedMessage);
                }
            }
            catch
            {
                Disconnected?.Invoke(this, _client);
            }
        }

        public void Write(string message)
        {
            try
            {
                _writer.WriteLine(message);
            }
            catch
            {
            }
        }
        public event EventHandler<string> MessageRecieved;
    }
}
