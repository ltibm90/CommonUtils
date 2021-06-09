using CommonUtils.Classes;
using CommonUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public class CommandParamHeadersList : AbstractList<CommandParamHeaderList>
    {
        public bool HasError
        {
            get
            {
                return !ErrorMsg.IsEmpty();
            }
        }
        public string ErrorMsg { get; set; }
    }
}
