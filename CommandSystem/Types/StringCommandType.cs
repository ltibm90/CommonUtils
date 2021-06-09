using CommonUtils.CommandSystem.Attributes;
using CommonUtils.CommandSystem.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Types
{
    public class StringCommandType : CommandType
    {
        public override CommandItemConvertResult Convert(CommandItem item, CommandExecuteData data)
        {
            var result = new CommandItemConvertResult();
            var attributes = item.CommandHeader.Attributes as StringTypeAttribute;
            if (attributes.MinUsed && attributes.MaxUsed && (item.InputValue.Length < attributes.Min || item.InputValue.Length > attributes.Max))
            {
                result.ErrorMessage = item.Container.GetText("ERR_VALUELEN_BETWEEN", attributes.Min, attributes.Max);
                return result;
            }
            else if (attributes.MinUsed && item.InputValue.Length < attributes.Min)
            {
                result.ErrorMessage = item.Container.GetText("ERR_VALUELEN_MIN", attributes.Min);
                return result;
            }
            else if (attributes.MaxUsed && item.InputValue.Length > attributes.Max)
            {
                result.ErrorMessage = item.Container.GetText("ERR_VALUELEN_MAX", attributes.Max);
                return result;
            }
            result.Success = true;
            result.Value = CommandValue.InitS(item.InputValue);

            return result;
        }
    }
}
