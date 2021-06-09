using CommonUtils.CommandSystem.Attributes;
using CommonUtils.CommandSystem.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem
{
    public abstract class CommandType
    {
        public abstract CommandItemConvertResult Convert(CommandItem item, CommandExecuteData data);
    }
}
