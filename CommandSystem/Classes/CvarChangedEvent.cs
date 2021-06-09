using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public class CvarChangedEvent
    {
        public CvarParam Cvar { get; set; }
        public CommandValue OldValue { get; set; }
        public CommandValue NewValue { get; set; }
        public CommandExecuteData ExecuteData { get; set; }
    }
}
