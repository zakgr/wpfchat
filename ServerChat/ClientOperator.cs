using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerChat
{
    class ClientOperator
    {
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;
        public ClientOperator(TcpClient client) {
            _client = client;
            _reader = new StreamReader(client.GetStream());
            _writer = new StreamWriter(client.GetStream());
            _writer.AutoFlush = true;
            StartReading();
        }
        private void StartReading() {
            var thread = new Thread(()=> {
                string recievedMessage = null;
                while ((recievedMessage=_reader.ReadLine())!=null) {
                    MessageRecieved?.Invoke(this, recievedMessage);
                }
            });
            thread.Start();
        }
        public void Write(string message) {
            _writer.WriteLine(message);
        }
        public event EventHandler<string> MessageRecieved;
    }
}
