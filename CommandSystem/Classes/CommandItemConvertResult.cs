using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public class CommandItemConvertResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public CommandValue Value { get; set; }
    }
}
