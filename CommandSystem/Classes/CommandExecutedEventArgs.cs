using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public class CommandExecutedEventArgs
    {
        public CommandExecutedEventArgs(CommandBase command)
        {
            this.Command = command;
        }
        public CommandExecutedEventArgs(CommandBase command, object data)
        {
            this.Command = command;
            this.CommandData = data;
        }
        public CommandBase Command { get; set; }
        public object CommandData { get; set; }
        public bool Cancel { get; set; }
        public CommandExecuteData ExecuteData { get; set; }
    }
}
