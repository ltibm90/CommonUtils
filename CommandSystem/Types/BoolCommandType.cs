using CommonUtils.CommandSystem.Attributes;
using CommonUtils.CommandSystem.Classes;
using CommonUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Types
{
    public class BoolCommandType : CommandType
    {
        public override CommandItemConvertResult Convert(CommandItem item, CommandExecuteData data)
        {
            var result = new CommandItemConvertResult();
            if (!item.InputValue.IsBool())
            {
                result.ErrorMessage = item.Container.GetText("ERR_VALUE_BOOL");
            }
            else
            {
                result.Value = CommandValue.InitS(item.InputValue.ToBool());
                result.Success = true;
            }
            return result;
        }
    }
}
