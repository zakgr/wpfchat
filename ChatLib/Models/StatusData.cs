using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLib.Models
{


    /// <summary>
    /// Sent by server to clients when a client's status changes.
    /// </summary>
    public class ClientStatusChanged : BaseCommand
    {
        public string Status { get; set; }
        public string Username { get; set; }
    }


    /// <summary>
    /// Sent to all clients to report online users.
    /// </summary>
    public class StatusReport : BaseCommand
    {
        public List<string> Usernames { get; set; }
    }

    public class BroadcastMessage : BaseCommand
    {
        public string Message { get; set; }
        public string Username { get; set; }
    }

    public class RoomMessage : BaseCommand
    {
        public string Message { get; set; }
        public string Room { get; set; }

    }

    public class CreateRoom : BaseCommand
    {
        public List<string> Users { get; set; }

    }
}
