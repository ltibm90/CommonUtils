using CommonUtils.CommandSystem.Attributes;
using CommonUtils.CommandSystem.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public class CvarParam : CommandBase
    {
        public Func<CvarChangedEvent, CommandExecute> OnCvarChanged { get; set; }
        private CommandParamHeader header;
        public CommandParamHeader Header
        {
            get
            {
                if (this.header == null)
                {
                    this.header = new CommandParamHeader();
                    this.header.Command = this;
                    this.header.TypeString = "string";
                }
                return this.header;
            }
            set
            {
                this.header = value;
            }
        }
        public CommandValue Value { get; set; }
        public string DefaultValue { get; set; }
        public override CommandBaseType BaseType
        {
            get
            {
                return CommandBaseType.CT_CVAR;
            }
        }

        public override CommandExecute Execute(string content, CommandExecuteData data)
        {
            lock (this)
            {
                var executeResult = new CommandExecute();
                string error = null;
                CommandValue newValue = null;
                this.ConvertAndValidate(content, out error, out newValue, data);
                if (error == null)
                {
                    CommandExecute er2 = null;
                    var cvarevent = new CvarChangedEvent() { Cvar = this, OldValue = this.Value, NewValue = newValue, ExecuteData = data };
                    CommandExecutedEventArgs execEv = new CommandExecutedEventArgs(this, cvarevent);
                    execEv.ExecuteData = data;
                    this.Container.OnCommandExecutedPre(execEv);
                    if (execEv.Cancel)
                    {
                        executeResult.Message = this.Container.GetText("ERR_CMD_CANCELLED");
                        executeResult.Result = CommandExecuteResult.CER_EXECUTE_CANCEL;
                        return executeResult;
                    }
                    if (this.OnCvarChanged != null)
                    {
                        er2 = this.OnCvarChanged?.Invoke(cvarevent);
                    }
                    if (er2 == null || er2.Result == CommandExecuteResult.CER_EXECUTE_SUCCESS)
                    {
                        this.Value = newValue;
                        executeResult.Message = this.Container.GetText("CMD_CVAR_CHANGED", this.Name, newValue.ToString());
                        executeResult.Result = CommandExecuteResult.CER_EXECUTE_SUCCESS;
                    }
                    else
                    {
                        executeResult.Message = er2.Message;
                        if (string.IsNullOrEmpty(executeResult.Message))
                        {
                            executeResult.Message = "!error";
                        }
                    }
                    this.Container.OnCommandExecutedPost(execEv);
                }
                else
                {
                    executeResult.Message = error;
                }
                return executeResult;
            }

        }

        private bool ConvertAndValidate(string input, out string error, out CommandValue newValue, CommandExecuteData data)
        {
            newValue = null;
            error = null;
            CommandType itemType = null;
            CommandItem item = new CommandItem();
            item.CommandHeader = this.Header;
            item.CommandValue = new CommandValueItem();
            item.CommandValue.Values.Add(input);
            if (this.Header.CommandType != null) itemType = this.Header.CommandType.Invoke();
            var result = itemType.Convert(item, data);
            if (!result.Success)
            {
                error = result.ErrorMessage;
                if (string.IsNullOrEmpty(error))
                {
                    error = "error!";
                }
                return false;
            }
            else
            {
                CustomValidateContent content = null;
                if (this.Container.OnConvert != null)
                {
                    content = new CustomValidateContent();
                    content.Header = this.Header;
                    content.Value = result.Value;
                    content.ExecuteData = data;
                    var cr = this.Container.OnConvert(content);
                    if(!cr.Success)
                    {
                        error = cr.Message;
                        if (string.IsNullOrEmpty(error))
                        {
                            error = "error!";
                        }
                        return false;
                    }
                }
                if (this.CustomConvert != null)
                {
                    if(content == null)
                    {
                        content = new CustomValidateContent();
                        content.Header = this.Header;
                        content.Value = result.Value;
                        content.ExecuteData = data;
                    }
                    var customResult = this.CustomConvert(content);
                    if (customResult != null)
                    {
                        if (!customResult.Success)
                        {
                            error = customResult.Message;
                            if (string.IsNullOrEmpty(error))
                            {
                                error = "error!";
                            }
                            return false;
                        }
    
                    }
                }
                newValue = result.Value;
            }

            return true;
        }
        public CvarParam Then(Action<CvarParam> action)
        {
            action(this);
            return this;
        }
    }
}
