using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public class CommandItemList : List<CommandItem>
    {
        public CommandItemList()
        {

        }
        public CommandItemList(int count) : base(count)
        {
            for (int i = 0; i < count; i++)
            {
                this.Add(null);
            }
        }
        public CommandValue this[string key]
        {
            get
            {
                int id = this.GetIdByName(key);
                if (id == -1) return null;
                return this[id].Value;
            }
        }
        private int GetIdByName(string name)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].CommandHeader.Name == name) return i;
            }
            return -1;
        }
    }
}
