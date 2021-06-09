using CommonUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Attributes
{
    public class FloatTypeAttribute : CommandParamHeaderAttributes
    {
        public float Min { get; set; }
        public float Max { get; set; }
        protected override CommandParamHeaderAttributeResult OnAttributeSet(string key, string invalue, out string outvalue)
        {
            outvalue = null;
            CommandParamHeaderAttributeResult result = CommandParamHeaderAttributeResult.CPHAR_Handled;
            if (key == "min")
            {
                this.Min = invalue.ToSingle();
            }
            else if (key == "max")
            {
                this.Max = invalue.ToSingle();
            }
            else
            {
                result = CommandParamHeaderAttributeResult.CPHAR_Cancel;
            }
            return result;
        }
    }
}
