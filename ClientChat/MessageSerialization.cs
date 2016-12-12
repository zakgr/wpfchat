using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ClientChat
{
    class MessageSerialization
    {
        private static StreamReader _reader;
        private static StreamWriter _writer;
        private TcpClient _client;
        private MessageStruct message;
        public MessageSerialization(TcpClient client)
        {
            _client = client;
            _reader = new StreamReader(_client.GetStream());
            _writer = new StreamWriter(_client.GetStream());
            _writer.AutoFlush = true;
            message = new MessageStruct();
            InitializeUser();
            StartReading();
            _client.Connect(message.IP, 3000);
            Write();
        }

        private void Write()
        {
            string sendMessage = null;
            while ((sendMessage = Console.ReadLine()) != null)
            {
                message.Date = DateTime.Now;
                message.Message = sendMessage;
                sendMessage = JsonConvert.SerializeObject(message);
                _writer.WriteLine(sendMessage);
            }
        }

        private void InitializeUser() {
            string configPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\config.ini";
            if (File.Exists(configPath))
            {
                string configFile = File.ReadAllText(configPath);
                message = JsonConvert.DeserializeObject<MessageStruct>(configFile);
            }
            else
            {
                Console.Write("Give a user name: ");
                message.UserName = Console.ReadLine();
                Console.Write("Give Ip Adress of Server: ");
                message.IP = Console.ReadLine();
                using (StreamWriter writer = new StreamWriter(configPath, false, Encoding.Default))
                {
                    writer.Write(JsonConvert.SerializeObject(message));
                }
            }
            message.PID = Process.GetCurrentProcess().Id;
        }
        private void StartReading()
        {
            var thread = new Thread(() => {
                string recievedMessage = null;
                MessageStruct input = new MessageStruct();
                while ((recievedMessage = _reader.ReadLine()) != null)
                {
                    input = JsonConvert.DeserializeObject<MessageStruct>(recievedMessage);
                    var output = $"{input.UserName } <{input.PID}>({input.Date}) ";
                    Console.WriteLine(output);

                }
            });
            thread.Start();
        }
    }
}