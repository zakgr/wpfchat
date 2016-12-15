using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLib.Models
{

    public class CommandWrapper
    {
        public Type Type { get; set; }

        public object Overhead { get; set; }

    }

    public abstract class BaseCommand
    {
        protected BaseCommand()
        {
            DateTime = DateTime.Now;
        }

        public DateTime DateTime { get; set; }
    }
}
