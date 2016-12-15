﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ChatLib.Models;
using Newtonsoft.Json;

namespace ChatLib
{
    public class ChatClient:CommandHandler
    {
        private readonly IPAddress _address;
        private readonly int _pid;
        private readonly int _portno;
        private readonly string _username;
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;
        public string Status;

        private List<string> _usernames(List<string> u)
        {
            if (u == null | u?.Count == 0)
            {
                u = new List<string>();
                u.Add("All");
            }
            else
            {
                var localuser = _username + "@" + _client.Client.LocalEndPoint.ToString();
                if (!u.Any(user => user.Contains(localuser))) u.Add(localuser);
            }
            return u;
        } 
        public ChatClient(IPAddress address, int portno, string username)
        {
            _address = address;
            _portno = portno;
            _username = username;
            _pid = Process.GetCurrentProcess().Id;
        }


        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler Connecting;

        public async Task Connect()
        {
            _client = new TcpClient();
            Connecting?.Invoke(this, EventArgs.Empty);
            while (!_client.Connected)
                try
                {
                    await _client.ConnectAsync(_address.ToString(), _portno);
                    Connected?.Invoke(this, EventArgs.Empty);
                    _reader = new StreamReader(_client.GetStream());
                    _writer = new StreamWriter(_client.GetStream()) { AutoFlush = true };
                    StartReading();
                    Write(new ClientStatusChanged()
                    {
                        Status = "online",
                        Username = _username
                    });
                }
                catch (Exception)
                {
                    await Task.Delay(5000);
                }
        }

        private void Write(BaseCommand msg)
        {
            try
            {
                var json = JsonConvert.SerializeObject(new CommandWrapper() { Type = msg.GetType(), Overhead  = msg});
                _writer.WriteLine(json);
            }
            catch (Exception)
            {
                Disconnected?.Invoke(this, EventArgs.Empty);
                var t = Connect();
            }
        }

        public void CreateRoom(string sendMessage,List<string> usernames)
        {
            //var userlist = _usernames(usernames);
            //var message = new MessageInfo
            //{
            //    Date = DateTime.Now,
            //    Message = "",
            //    Type = CommandType.Room,
            //    UserName = _username,
            //    UsersRecipient = userlist
            //};
            //Write(message);

        }

        public void SendBroadcastMessage(string sendMessage)
        {
            var command =  new BroadcastMessage()
            {
                Message = sendMessage
            };
            Write(command);
        }


        private async void StartReading()
        {
            try
            {
                string recievedMessage = null;
                Console.WriteLine("Listening");
                while ((recievedMessage = await _reader.ReadLineAsync()) != null)
                {
                    var input = JsonConvert.DeserializeObject<CommandWrapper>(recievedMessage);
                    var command = JsonConvert.DeserializeObject(input.Overhead.ToString(), input.Type) as BaseCommand;
                    Invoke(_client, command);
                }
            }
            catch
            {
                Disconnected?.Invoke(this, EventArgs.Empty);
                await Connect();
            }
        }
    }
}