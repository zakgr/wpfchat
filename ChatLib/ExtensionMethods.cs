using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatLib
{
    static class ExtensionMethods
    {
        public static string FindByClient(this Dictionary<string, ClientOperator> dict, TcpClient client)
        {
            return dict.First(kv => kv.Value.TcpClient == client).Key;
        }
    }
}
