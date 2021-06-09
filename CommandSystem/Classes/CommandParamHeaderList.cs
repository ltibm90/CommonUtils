using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public class CommandParamHeaderList : List<CommandParamHeader>
    {
        public CommandParamHeaderList Presets { get; set; }
        public string ErrorMsg { get; set; }
        public bool HasError
        {
            get
            {
                return !string.IsNullOrEmpty(this.ErrorMsg);
            }
        }
        public new CommandParamHeader this[int index]
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
        public void ReCountOptional()
        {
            this.optionalcount = 0;
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].HasDefaultValue) this.optionalcount++;
            }
        }
        public void ReCountCell()
        {
            this.cellcount = 0;
            for (int i = 0; i < this.Count; i++)
            {
                if(this[i].Attributes.Cell > 1)
                {
                    this.cellcount += this[i].Attributes.Cell - 1;
                }
            }
        }
        private int optionalcount = 0;
        public int OptionalCount
        {
            get
            {
                if(this.optionalcount == 0 && this.Count > 0)
                {
                    this.ReCountOptional();
                }
                return this.optionalcount;
            }
        }
        private int cellcount = 0;
        public int CellCount
        {
            get
            {
                if (this.cellcount == 0 && this.Count > 0)
                {
                    this.ReCountCell();
                }
                return this.cellcount;
            }
        }
        private string oldUsage = "";
        public string GetUsageMesage()
        {
            if (oldUsage == "")
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" ");
                for (int i = 0; i < this.Count; i++)
                {
                    var header = this[i];
                    if (!header.HasDefaultValue)
                    {
                        sb.Append("(" + header.TypeString + ")" + header.Name);
                    }
                    else
                    {
                        sb.Append("[(" + header.TypeString + ")" + header.Name + "]");
                    }
                    sb.Append(" ");

                }
                oldUsage = sb.ToString();
            }
            return oldUsage;
        }

    }
}
