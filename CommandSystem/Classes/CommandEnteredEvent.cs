using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public class CommandEnteredEvent : EventArgs
    {
        public string Name { get; set; }
        public string Parameter { get; set; }
        public bool Cancel { get; set; }
        public bool Handled { get; set; }
        public CommandExecuteData ExecuteData { get; set; }
        public CommandEnteredEvent(string name, string parameter)
        {
            this.Name = name;
            this.Parameter = parameter;

        }
    }
}
