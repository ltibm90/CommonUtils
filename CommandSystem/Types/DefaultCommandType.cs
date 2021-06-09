using CommonUtils.CommandSystem.Attributes;
using CommonUtils.CommandSystem.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Types
{
    public class DefaultCommandType : CommandType
    {
        public override CommandItemConvertResult Convert(CommandItem item, CommandExecuteData data)
        {
            var result = new CommandItemConvertResult();
            result.Success = true;
            result.Value = CommandValue.InitS(item.InputValue);
            return result;
        }
    }
}
