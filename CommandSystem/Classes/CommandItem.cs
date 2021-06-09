using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public class CommandItem
    {
        public CommandParamHeader CommandHeader { get; set; }
        public CommandValueItem CommandValue { get; set; }
        public CommandContainer Container
        {
            get
            {
                return this.Command.Container;
            }
        }
        public CommandBase Command
        {
            get
            {
                return this.CommandHeader.Command;
            }
        }
        public CommandValue Value { get; set; }
        public string InputValue
        {
            get
            {
                return this.CommandValue.Value;
            }
        }

    }
}
