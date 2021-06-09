using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public class CustomValidateContent
    {
        public CustomValidateContent()
        {

        }
        public int ParamIndex { get; set; }
        public string ParamName { 
            get 
            {
                return this.Header.Name;
            } 
        }
        public string Input { get; set; }
        public CommandBase Command
        {
            get
            {
                return this.Header.Command;
            }
        }
        public CommandValue Value { get; set; }
        public CommandParamHeader Header { get; set; }
        public CommandExecuteData ExecuteData { get; set; }
    }
}
