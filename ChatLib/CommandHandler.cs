using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatLib.Models;

namespace ChatLib
{
    public abstract class CommandHandler
    {
        private readonly List<ActionSet> _commands;


        protected CommandHandler()
        {
            _commands = new List<ActionSet>();
        }

        

        public void On<T>(Action<object, T> action) where T:BaseCommand
        {
         
            _commands.Add(new ActionSet()
            {
                Action = action,
                ActionWrapper = (sender, data)=>action(sender, (T)data),
                Type = typeof(T)
            });
        }

        protected void Invoke<T>(object sender, T data) where T:BaseCommand
        {
            _commands.Where(com => com.Type == data.GetType()).ToList().ForEach(act =>
            {
                act.ActionWrapper?.Invoke(sender, data);
            });
        }

    }

    internal class ActionSet
    {
        public Type Type { get; set; }

        public Action<object, BaseCommand> ActionWrapper { get; set; }

        public object Action { get; set; }

    }
}
