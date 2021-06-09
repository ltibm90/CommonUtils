using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public enum CommandBaseType
    {
        CT_CVAR = 0,
        CT_COMMAND
    }
    public abstract class CommandBase
    {
        public bool Enabled { get; set; } = true;
        public string Name { get; set; }
        public CommandContainer Container { get; set; }
        public abstract CommandBaseType BaseType { get; }
        
        public abstract CommandExecute Execute(string content, CommandExecuteData data);
        public CommandParam ToCommand()
        {
            if(this.BaseType == CommandBaseType.CT_COMMAND)
            {
                return (CommandParam)this;
            }
            return null;
        }
        public CvarParam ToCvar()
        {
            if (this.BaseType == CommandBaseType.CT_CVAR)
            {
                return (CvarParam)this;
            }
            return null;
        }

        public Func<CustomValidateContent, CustomConvertResult> CustomConvert { get; set; }
        public int Flags { get; set; }

    }
}
