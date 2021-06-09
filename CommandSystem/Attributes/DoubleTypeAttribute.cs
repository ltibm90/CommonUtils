using CommonUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Attributes
{
    public class DoubleTypeAttribute : CommandParamHeaderAttributes
    {
        public double Min { get; set; }
        public double Max { get; set; }
        protected override CommandParamHeaderAttributeResult OnAttributeSet(string key, string invalue, out string outvalue)
        {
            outvalue = null;
            CommandParamHeaderAttributeResult result = CommandParamHeaderAttributeResult.CPHAR_Cancel;
            if (key == "min")
            {
                this.MinUsed = true;
                this.Min = invalue.ToDouble();
            }
            else if (key == "max")
            {
                this.MaxUsed = true;
                this.Max = invalue.ToDouble();
            }
            else
            {
                result = CommandParamHeaderAttributeResult.CPHAR_NoAction;
            }
            return result;
        }
    }
}
