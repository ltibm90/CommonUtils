using CommonUtils.CommandSystem.Attributes;
using CommonUtils.CommandSystem.Classes;
using CommonUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Types
{
    public class DoubleCommandType : CommandType
    {
        public override CommandItemConvertResult Convert(CommandItem item, CommandExecuteData data)
        {
            var result = new CommandItemConvertResult();
            if (!item.InputValue.IsNumeric())
            {
                result.ErrorMessage = item.Container.GetText("ERR_VALUE_NUM");
            }
            else
            {
                double intResult = item.InputValue.ToDouble();
                var attributes = item.CommandHeader.Attributes as DoubleTypeAttribute;
                if (attributes.MinUsed && attributes.MaxUsed && (intResult < attributes.Min || intResult > attributes.Max))
                {
                    result.ErrorMessage = item.Container.GetText("ERR_VALU_BETWEEN", attributes.Min, attributes.Max);
                    return result;
                }
                else if (attributes.MinUsed && intResult < attributes.Min)
                {
                    result.ErrorMessage = item.Container.GetText("ERR_VALUE_MIN", attributes.Min);
                    return result;
                }
                else if (attributes.MaxUsed && intResult > attributes.Max)
                {
                    result.ErrorMessage = item.Container.GetText("ERR_VALUE_MAX", attributes.Max);
                    return result;
                }
                result.Value = CommandValue.InitS(intResult);
                result.Success = true;
            }
            return result;
        }
    }
}
