using CommonUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Attributes
{
    public class IntTypeAttribute : CommandParamHeaderAttributes
    {
        public int Min { get; set; }
        public int Max { get; set; }
        protected override CommandParamHeaderAttributeResult OnAttributeSet(string key, string invalue, out string outvalue)
        {
            outvalue = null;
            CommandParamHeaderAttributeResult result = CommandParamHeaderAttributeResult.CPHAR_Cancel;
            if (key == "min")
            {
                this.MinUsed = true;
                this.Min = invalue.ToInt32();
            }
            else if (key == "max")
            {
                this.MaxUsed = true;
                this.Max = invalue.ToInt32();
            }
            else
            {
                result = CommandParamHeaderAttributeResult.CPHAR_NoAction;
            }
            return result;
        }
    }
}
