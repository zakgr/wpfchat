using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ClientChat
{
    public class MessageSerialization
    {
        private static StreamReader _reader;
        private static StreamWriter _writer;
        private MessageStruct _message;
        public MessageSerialization(TcpClient client)
        {
            _message = new MessageStruct();
            InitializeUser();
            client.Connect(_message.Ip, 3000);
            _reader = new StreamReader(client.GetStream());
            _writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
            StartReading();
            Write();
        }

        private void Write()
        {
            string sendMessage = null;
            while ((sendMessage = Console.ReadLine()) != null)
            {
                _message.Date = DateTime.Now;
                _message.Message = sendMessage;
                sendMessage = JsonConvert.SerializeObject(_message);
                _writer.WriteLine(sendMessage);
            }
        }

        private void InitializeUser()
        {
            var configPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\config.ini";
            if (File.Exists(configPath))
            {
                var configFile = File.ReadAllText(configPath);
                _message = JsonConvert.DeserializeObject<MessageStruct>(configFile);
            }
            else
            {
                Console.Write("Give a user name: ");
                _message.UserName = Console.ReadLine();
                Console.Write("Give Ip Adress of Server: ");
                _message.Ip = Console.ReadLine();
                using (var writer = new StreamWriter(configPath, false, Encoding.Default))
                {
                    writer.Write(JsonConvert.SerializeObject(_message));
                }
            }
            _message.Pid = Process.GetCurrentProcess().Id;
        }
        private static void StartReading()
        {
            var thread = new Thread(() =>
            {
                try
                {
                    string recievedMessage = null;
                    while ((recievedMessage = _reader.ReadLine()) != null)
                    {
                        var input = JsonConvert.DeserializeObject<MessageStruct>(recievedMessage);
                        var output = $"{input.UserName} <{input.Pid}> ({input.Date.Hour}) send '{input.Message}' ";
                        Console.WriteLine(output);
                    }
                }
                catch
                {
                    Console.WriteLine("Error Server Connection");
                }
            });
            thread.Start();
        }
    }
}