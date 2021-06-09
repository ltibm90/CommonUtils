using CommonUtils.CommandSystem.Classes;
using CommonUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Types
{
    class PointCommandType : CommandType
    {
        public override CommandItemConvertResult Convert(CommandItem item, CommandExecuteData data)
        {
            var result = new CommandItemConvertResult();
            string pt = "";
            if(item.CommandValue.Values.Count > 1)
            {
                pt = string.Join(" ", item.CommandValue.Values);
            }
            else
            {
                pt = item.InputValue;
            }
            if(item.CommandValue.PreValidated || pt.IsPoint())
            {
                result.Success = true;
                result.Value = CommandValue.InitS(pt.ToPoint());
            }
            return result;
        }
    }
}
