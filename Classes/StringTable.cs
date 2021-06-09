using CommonUtils.Extensions;
using System.Collections.Generic;

namespace CommonUtils.Classes
{
    public class StringTableDictionary : Dictionary<string, StringTable>
    {
        public string Name { get; set; }
        public StringTableContainer Container { get; internal set; }
        public StringTableDictionary()
        {

        }
        public StringTableDictionary(string name)
        {
            this.Name = name;
        }
        public StringTable GetTable(string header)
        {
            StringTable table = null;
            this.TryGetValue(header, out table);
            return table;
        }
        public StringTable AddTable(string name, string content)
        {
            StringTable table = new StringTable(content);
            this[name] = table;
            return table;
        }
        /// <summary>
        /// NAME = VALUE;
        /// </summary>
        /// <param name="content"></param>
        public void AddFromString(string content)
        {
            int iof = content.IndexOf('=');
            if (iof == content.Length - 1) return;
            if (iof == -1) return;
            string name = content.Substring(0, iof).Trim();
            string value = content.Substring(iof + 1).Trim();
            if(value.IncludingQuote())
            {
                value = value.RemoveQuote();
            }
            this.AddTable(name, value);

        }

        public void AddFromStringMultiline(string content)
        {
            StringSplitter spt = new StringSplitter(content, "\r\n");
            spt.SplitQuoteOption = StringQuoteOption.DoubleQuote;
            spt.SplitOptions = StringSplitOption.AllowSpecialChar | StringSplitOption.PrintQuote | StringSplitOption.TrimPerElement;

            var r = spt.Split();
            for (int i = 0; i < r.Length; i++)
            {
                this.AddFromString(r[i]);
            }
        }
        public string GetString(string header, params object[] fmt)
        {
            StringTable table = this.GetTable(header);
            if (table == null) return header;
            return table.Format(fmt);
        }
    }
    public class StringTable
    {
        public StringTable()
        {

        }
        public StringTable(string content)
        {
            this.Content = content;
        }
        public string Content { get; set; }
        public string Format(params object[] fmt)
        {
            return string.Format(this.Content, fmt);
        }
    }
    public class StringTableContainer
    {

        private StringTableDictionary lastTable = null;
        private int lastTableId = -1;
        public StringTableContainer()
        {
            this.tables = new List<StringTableDictionary>();

        }


        public StringTableDictionary this[string name]
        {
            get
            {
                if (lastTable != null && lastTable.Name == name) return lastTable;
                return this[this.GetTableIdByname(name)];
            }
        }
        public StringTableDictionary this[int index]
        {
            get
            {
                if (index == lastTableId) return this.lastTable;
                if (index == -1 || index >= this.tables.Count)
                {
                    if(this.DefaultTableId >= 0 && this.DefaultTableId < this.tables.Count)
                    {
                        index = this.DefaultTableId;
                    }
                    else return null;
                }
                var item = this[index];
                lastTable = item;
                lastTableId = -1;
                return item;
            }
        }
        private List<StringTableDictionary> tables;

        public int DefaultTableId { get; set; }
        public string GetString(int tableid, string header, params object[] fmt)
        {
            return this.GetString(tableid, header, true, fmt);
        }
        public string GetString(int tableid, string header, bool returnDefault, params object[] fmt)
        {
            if ((tableid < 0 || tableid >= this.tables.Count) && returnDefault) tableid = this.DefaultTableId;
            if (tableid < 0 || tableid >= this.tables.Count) return header;
            StringTable table = this.tables[tableid].GetTable(header);
            if (table != null) return table.Format(fmt);
            if (returnDefault && this.DefaultTableId >= 0 && this.DefaultTableId < this.tables.Count)
            {
                table = this.tables[this.DefaultTableId].GetTable(header);
                if (table != null) return table.Format(fmt);


            }
            return header;
        }
        public string GetString(string name, string header)
        {
            return this.GetString(name, header, true);
        }
        public string GetString(string name, string header, bool returnDefault)
        {
            int id = -1;
            if (this.lastTable != null && this.lastTable.Name == name) id = this.lastTableId;
            if (id == -1) id = this.GetTableIdByname(name);
            return this.GetString(id, header, returnDefault);
        }
        public void AddTable(StringTableDictionary table)
        {
            this.tables.Add(table);
            table.Container = this;
        }
        public int GetTableIdByname(string name)
        {
            for (int i = 0; i < this.tables.Count; i++)
            {
                if (this.tables[i].Name == name) return i;
            }
            return -1;
        }
        public void RemoveTableAt(int id)
        {
            if (id < 0 || id >= this.tables.Count) return;
            if(this.DefaultTableId > id)
            {
                this.DefaultTableId--;
            }
            this.tables[id].Container = null;
            this.tables.RemoveAt(id);
            lastTable = null;
            lastTableId = -1;
        }
        public bool RemoveTable(StringTableDictionary table)
        {
            table.Container = null;
            if(this.tables.Remove(table))
            {
                lastTable = null;
                lastTableId = -1;
                return true;
            }
            return false;
        }

    }
}
