using CommonUtils.CommandSystem.Attributes;
using CommonUtils.CommandSystem.Classes;
using CommonUtils.CommandSystem.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem
{
    public class CommandTypeOption
    {
        public CommandTypeOption()
        {

        }
        public CommandTypeOption(Func<CommandType> typeClass)
        {
            this.TypeClass = typeClass;
        }
        public CommandTypeOption(Func<CommandType> typeClass, Func<CommandParamHeaderAttributes> attributeClass)
        {
            this.TypeClass = typeClass;
            this.AttributeClass = attributeClass;
        }
        public CommandContainer Container { get; set; }
        public Func<CommandType> TypeClass { get; set; }
        private Func<CommandParamHeaderAttributes> attributeClass;
        public Func<CommandParamHeaderAttributes> AttributeClass
        {
            get
            {
                if (this.attributeClass == null && this.Container != null)
                {
                    return this.Container.DefaultCommandType.AttributeClass;
                }
                return this.attributeClass;
            }
            set
            {
                this.attributeClass = value;
            }
        }
    }
}
