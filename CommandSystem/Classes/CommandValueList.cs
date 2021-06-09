using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public class CommandValueList : List<CommandValueItem>
    {
        public new CommandValueItem this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count) return null;

                return base[index];
            }
            set
            {
                if (index < 0 || index >= this.Count) return;
                base[index] = value;
            }
        }
        public int GetIdByName(string name)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Name == name) return i;
            }
            return -1;
        }
    }
}
