using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLib.Models
{
    public class Command<T>
    {
        public CommandType CommandType { get; set; }

        public DateTime DateTime { get; set; }

        public T Data { get; set; }
    }

    static class Command
    {
        public static Command<T> CreateCommand<T>(CommandType type, T t)
        {
            return new Command<T>()
            {
                CommandType = type,
                DateTime = DateTime.Now,
                Data = t
            };
        }
    }
}
