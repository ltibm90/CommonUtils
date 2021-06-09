using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{

    public class CommandValueItem
    {
        public CommandValueItem()
        {
            this.Values = new List<string>();
        }
        public string Name { get; set; }
        public string Value
        {
            get
            {
                if (this.Values.Count <= 0) return null;
                return this.Values[0];
            }
        }
        public List<string> Values { get; set; }
        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(this.Value);
            }
        }
        public bool HasName
        {
            get
            {
                return !string.IsNullOrEmpty(this.Name);
            }
        }
        public bool PreValidated { get; set; }
    }
}
