using CommonUtils.CommandSystem.Classes;
using CommonUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem
{
    public class CommandParamHeaderAttributes
    {
        private Dictionary<string, string> inner;
        public CommandParamHeaderAttributes()
        {
            this.inner = new Dictionary<string, string>();
        }
        public string this[string key]
        {
            get
            {
                return this.Get(key);
            }
            set
            {
                this.Set(key, value);
            }
        }
        public bool Delete(string key)
        {
            return this.inner.Remove(key);
        }
        public bool Exists(string key)
        {
            return this.inner.ContainsKey(key);
        }
        public string Get(string key, string defaultValue = null)
        {
            string outvalue;
            if (this.inner.TryGetValue(key, out outvalue))
            {
                return outvalue;
            }
            return defaultValue;
        }
        public void Set(string key, string value)
        {
            string outvalue;

            var result = this.OnAttributeSet(key, value, out outvalue);
            if (result == CommandParamHeaderAttributeResult.CPHAR_Cancel) return;
            if (result == CommandParamHeaderAttributeResult.CPHAR_Handled) value = outvalue;
            if (key == "cell")
            {
                this.Cell = value.ToInt32();
                return;
            }
            this.inner[key] = value;
        }
        public bool MinUsed { get; set; }
        public bool MaxUsed { get; set; }
        public int Cell { get; set; }
        public Func<CommandValueList, int, int> OnGetCellFor { get; set; }
        public virtual int GetCellFor(CommandValueList values, int start)
        {
            if (this.OnGetCellFor != null) return this.OnGetCellFor(values, start);
            return this.Cell;
        }
        protected virtual CommandParamHeaderAttributeResult OnAttributeSet(string key, string invalue, out string outvalue)
        {
            outvalue = null;
            return CommandParamHeaderAttributeResult.CPHAR_NoAction;
        }
    }
}
