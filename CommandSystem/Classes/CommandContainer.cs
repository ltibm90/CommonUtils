using CommonUtils.Classes;
using CommonUtils.CommandSystem.Attributes;
using CommonUtils.CommandSystem.Types;
using CommonUtils.CommandSystem.Utils;
using CommonUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public class CommandContainer
    {
        public StringTableDictionary StringTable { get; set; }
        public CommandContainer()
        {
            this.StringTable = new StringTableDictionary();
            this.StringTable.AddFromStringMultiline(@"ERR_CMD_NOT_FOUND = Command not found: {0}
ERR_CMD_PARAM_REQ = Command paramater '{0}' is required
ERR_CMD_MULTI_VAL = You can't enter multiple value a parameter
ERR_CMD_INVALID = Invalid parameter: {0}
ERR_VALUELEN_BETWEEN = Value length must be between {0} and {1}
ERR_VALUELEN_MAX = Max value length must be {0}
ERR_VALUELEN_MIN = Min value length must be {0}
ERR_VALUE_BETWEEN = Value must be between {0} and {1}
ERR_VALUE_MAX = Max value must be {0}
ERR_VALUE_MIN = Min value must be {0}
ERR_VALUE_NUM = Value must be mumeric
ERR_VALUE_BOOL = Value must be boolen type
ERR_CMD_TYPE = Command parameter must have a type
ERR_CMD_DEFVAL = Command parameter must have a default value
ERR_CMD_NAME = Command parameter must have a name
CMD_CVAR_CHANGED = Cvar '{0}' changed to '{1}'
ERR_CMD_CANCELLED = Command execution cancelled
CMD_USAGE = Usage: {0}
ERR_CMD_PARAM = Entered parameters not enaught
ERR_CMD_PARAM_MIN = Mininum {0} parameter required
ERR_CMD_PARAM_MAX = You can't enter more than {0} pamateres
ERR_CMD_PARAM_POINT = Value must be point type");
            this.inner = new Dictionary<string, CommandBase>();
            this.TypeOptions = new Dictionary<string, CommandTypeOption>();
            this.InitDefaultTypes();

        }
        public event EventHandler<CommandExecutedEventArgs> CommandExecutedPre;
        public event EventHandler<CommandExecutedEventArgs> CommandExecutedPost;

        public event EventHandler<CommandEnteredEvent> CommandEnteredPre;
        public event EventHandler<CommandEnteredEvent> CommandEnteredPost;
        public Func<string, object[], string> OnGetText { get; set; }
        private Dictionary<string, CommandBase> inner;
        private Dictionary<string, CommandTypeOption> TypeOptions { get; set; }
        public CommandTypeOption DefaultCommandType { get; set; }
        public Func<CustomValidateContent, CustomConvertResult> OnConvert { get; set; }
        public string Prefix { get; set; }

        public string LastError { get; set; }

        public string GetText(string name, params object[] fmt)
        {
            if(this.OnGetText != null)
            {
                string val = this.OnGetText(name, fmt);
                if (val != null) return val;
            }
            return this.StringTable.GetString(name, fmt);
        }
        public void SetTypeOption(string key, CommandTypeOption value)
        {
            this.TypeOptions[key] = value;
        }
        public CommandTypeOption GetTypeOption(string key)
        {
            CommandTypeOption outvalue = null;
            if (this.TypeOptions.TryGetValue(key, out outvalue))
            {
                return outvalue;
            }
            return this.DefaultCommandType;
        }

        public void InitDefaultTypes()
        {
            this.DefaultCommandType = new CommandTypeOption();
            this.DefaultCommandType.AttributeClass = () => new CommandParamHeaderAttributes();
            this.DefaultCommandType.TypeClass = () => new DefaultCommandType();
            this.SetTypeOption("string", new CommandTypeOption(() => new StringCommandType(), () => new StringTypeAttribute()));
            this.SetTypeOption("int", new CommandTypeOption(() => new IntCommandType(), () => new IntTypeAttribute()));
            this.SetTypeOption("float", new CommandTypeOption(() => new FloatCommandType(), () => new FloatTypeAttribute()));
            this.SetTypeOption("double", new CommandTypeOption(() => new DoubleCommandType(), () => new DoubleTypeAttribute()));
            this.SetTypeOption("bool", new CommandTypeOption(() => new BoolCommandType()));
            this.SetTypeOption("point", new CommandTypeOption(() => new PointCommandType(), () => new CommandParamHeaderAttributes()
            {
                Cell = 2,
                OnGetCellFor = (cmds, i) =>
                {
                    string val = cmds[i].Value;
                    if (val.IsPoint())
                    {
                        cmds[i].PreValidated = true;
                        return 1;
                    }
                    return 2;
                }
            }));
        }
        private void AddCommand(CommandParam command)
        {
            if (string.IsNullOrEmpty(command.Name)) return;
            inner[command.Name] = command;
            this.LastError = null;

        }
        public CvarParam AddCvar(string name, string defaultValue)
        {
            return this.AddCvar(name, defaultValue, null, null);
        }
        public CvarParam AddCvar(string name, string defaultValue, string param)
        {
            return this.AddCvar(name, defaultValue, param, null);
        }
        public CvarParam AddCvar(string name, string defaultValue, string param, Func<CvarChangedEvent, CommandExecute> onChanged)
        {
            CvarParam cvar = new CvarParam();
            cvar.Container = this;
            cvar.Header = CommandUtils.ParseSingleHeader(param, cvar);
            cvar.Name = name;
            cvar.DefaultValue = defaultValue;

            if(defaultValue != null)
            {
                var er = cvar.Execute(defaultValue, null);
                if (er.Result != CommandExecuteResult.CER_EXECUTE_SUCCESS)
                {
                    cvar.Value = CommandValue.InitS(defaultValue);
                }
            }
            else
            {
                cvar.Value = new CommandValue();
            }
            cvar.OnCvarChanged = onChanged;
            this.AddCvar(cvar);
            return cvar;
                
        }
        private void AddCvar(CvarParam cvar)
        {
            if (string.IsNullOrEmpty(cvar.Name)) return;
            inner[cvar.Name] = cvar;
            this.LastError = null;
        }

        public CommandParam AddCommand(string name, Func<CommandItemContainer, CommandExecute> target)
        {
            return this.AddCommand(name, null, target);
        }
        public CommandParam AddCommand(string name, string cmdParams, Func<CommandItemContainer, CommandExecute> target)
        {
            return this.AddCommand(name, cmdParams, null, target);
        }
        public CommandParam AddCommand(string name, string cmdParams, string defaultParam, Func<CommandItemContainer, CommandExecute> target)
        {
            return this.AddCommand(name, cmdParams, defaultParam, true, target);
        }
        public CommandParam AddCommand(string name, string cmdParams, string defaultParam, bool acceptMoreParameters, Func<CommandItemContainer, CommandExecute> target)
        {
            if (string.IsNullOrEmpty(name)) return null;
            CommandParam command = new CommandParam();
            command.Name = name;
            command.Container = this;
            command.Headers = CommandUtils.ParseCommandHeaders(cmdParams, command);
            command.DefaultHeader = CommandUtils.ParseSingleHeader(defaultParam, command);
            command.AcceptMoreParameters = acceptMoreParameters;
            if (command.Headers.HasError)
            {
                this.LastError = command.Headers.ErrorMsg;
                return null;
            }
            command.Target = target;
            this.AddCommand(command);
            return command;
        }
        public CommandExecute ExecuteNonPrefix(string command)
        {
           return this.Execute(this.Prefix + command);
        }
        public CommandExecute Execute(string command, CommandExecuteData executedata = null)
        {
            var executeResult = new CommandExecute();
            if (!string.IsNullOrEmpty(this.Prefix))
            {
                if (!command.StartsWith(this.Prefix)) return executeResult;
                command = command.Substring(this.Prefix.Length);
            }
            string commandParam = "";
            string commandName = "";
            int sIndex = command.IndexOf(' ');
            if (sIndex >= 0)
            {
                commandName = command.Substring(0, sIndex);
            }
            else
            {
                commandName = command;
            }
            if (sIndex >= 0) commandParam = command.Substring(sIndex + 1);
            CommandEnteredEvent enteredEv = new CommandEnteredEvent(commandName, commandParam);
            enteredEv.ExecuteData = executedata;
            this.OnCommandEnteredPre(enteredEv);
            if(enteredEv.Cancel)
            {
                executeResult.Message = this.GetText("ERR_CMD_CANCELLED");
                return executeResult;
            }
            else if(enteredEv.Handled)
            {
                commandName = enteredEv.Name;
                commandParam = enteredEv.Parameter;
            }
            CommandBase cmd = null;
            this.inner.TryGetValue(commandName, out cmd);
            if (cmd == null || !cmd.Enabled)
            {
                executeResult.Message = this.GetText("ERR_CMD_NOT_FOUND", commandName);
                return executeResult;
            }

            var cr = cmd.Execute(commandParam, executedata);
            if (cr == null) cr = CommandExecute.Error();
            cr.Command = cmd;
            this.OnCommandEnteredPost(enteredEv);
            return cr;

        }
        public bool RemoveCommand(string commandName)
        {
            if (this.inner.ContainsKey(commandName))
            {
                this.inner.Remove(commandName);
                return true;
            }
            return false;
        }
        public void Clear()
        {
            this.inner.Clear();
        }
        public CommandBase GetCommand(string name)
        {
            CommandBase cmd = null;
            this.inner.TryGetValue(name, out cmd);
            return cmd;
        }
        protected virtual void OnCommandEnteredPre(CommandEnteredEvent e)
        {
            this.CommandEnteredPre?.Invoke(this, e);
        }
        protected virtual void OnCommandEnteredPost(CommandEnteredEvent e)
        {
            this.CommandEnteredPost?.Invoke(this, e);
        }
        internal virtual void OnCommandExecutedPre(CommandExecutedEventArgs e)
        {
            this.CommandExecutedPre?.Invoke(this, e);
        }
        internal virtual void OnCommandExecutedPost(CommandExecutedEventArgs e)
        {
            this.CommandExecutedPost?.Invoke(this, e);
        }
    }
}
