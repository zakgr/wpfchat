using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace ChatLib
{
    public class ClientOperator 
    {
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;
        private readonly TcpClient _client;
        public event EventHandler<TcpClient> Disconnected;

        public TcpClient TcpClient => _client;

        public ClientOperator(TcpClient client)
        {
            _client = client;
            _reader = new StreamReader(client.GetStream());
            _writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
           
        }

        public async void StartReading()
        {
            try
            {
                string recievedMessage = null;
                while ((recievedMessage = await _reader.ReadLineAsync()) != null)
                {
                    DataReceived?.Invoke(this, recievedMessage);
                }
            }
            catch(InvalidOperationException)
            {
                Disconnected?.Invoke(this, _client);
            }
            catch(IOException)
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
                // ignored
            }
        }
        public event EventHandler<string> DataReceived;
    }
}
