using CommonUtils.CommandSystem.Attributes;
using CommonUtils.CommandSystem.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public class CommandParamHeader
    {
        public CommandParamHeader()
        {

        }
        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
        private string typeString;
        public string TypeString
        {
            get
            {
                return this.typeString;
            }
            set
            {
                this.typeString = value;
                if (this.Command != null && this.Command.Container != null)
                {
                    var typeOption = this.Command.Container.GetTypeOption(value);
                    if (typeOption != null && typeOption.TypeClass != null)
                    {
                        this.cmdType = typeOption.TypeClass;
                    }
                    if (this.cmdType == null)
                    {
                        this.cmdType = this.Command.Container.DefaultCommandType.TypeClass;
                    }
                    if (typeOption != null && typeOption.AttributeClass != null)
                    {
                        this.Attributes = typeOption.AttributeClass();
                    }
                    if (this.Attributes == null)
                    {
                        this.Attributes = new CommandParamHeaderAttributes();
                    }


                }
            }
        }
        private Func<CommandType> cmdType;
        public Func<CommandType> CommandType { get { return this.cmdType; } }
        public bool HasDefaultValue { get; set; }
        public string DefaultValue { get; set; }
        public CommandParamHeaderAttributes Attributes { get; set; }
        public CommandBase Command { get; set; }
    }
}
