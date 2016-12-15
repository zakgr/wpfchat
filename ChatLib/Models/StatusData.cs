using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLib.Models
{

    public enum CommandType
    {
        StatusReport,
        StatusChange,
        BroadcastMessage,
        RoomMessage,
        CreateRoom,
        Invite
    }


    /// <summary>
    /// Sent by clients when their status changes.
    /// </summary>
    public class StatusData
    {
        public string Status { get; set; }
        public string Username { get; set; }
    }

    /// <summary>
    /// Sent by server to clients when a client's status changes.
    /// </summary>
    public class ClientStatusChangedData
    {
        public string Status { get; set; }
        public string Username { get; set; }
    }


    /// <summary>
    /// Sent to all clients to report online users.
    /// </summary>
    public class StatusReport
    {
        public List<string> Usernames { get; set; }
    }


    public class BroadcastMessage
    {
        public string Message { get; set; }
        public string Username { get; set; }
    }

    public class RoomMessage
    {
        public string Message { get; set; }
        public string Room { get; set; }

    }

    public class CreateRoom
    {
        public List<string> Users { get; set; }

    }
}
