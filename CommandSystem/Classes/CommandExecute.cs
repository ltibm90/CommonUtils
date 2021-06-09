using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public enum CommandExecuteResult
    {
        CER_EXECUTE_CANCEL = -1,
        CER_EXECUTE_ERROR,
        CER_EXECUTE_SUCCESS,
        CER_EXECUTE_HANDLED
    }
    public class CommandExecute
    {
        public CommandExecute()
        {

        }
        public CommandExecute(CommandExecuteResult result)
        {
            this.Result = result;
        }
        public CommandExecute(CommandExecuteResult result, string message)
        {
            this.Result = result;
            this.Message = message;
        }
        public CommandExecute(CommandExecuteResult result, string message, object data)
        {
            this.Result = result;
            this.Message = message;
            this.Data = data;
        }
        public CommandExecuteResult Result { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public CommandBase Command { get; internal set; }

        public static CommandExecute Handled()
        {
            return CommandExecute.Handled(null);
        }
        public static CommandExecute Handled(string msg)
        {
            return CommandExecute.Create(CommandExecuteResult.CER_EXECUTE_HANDLED, msg, null);
        }
        public static CommandExecute Error()
        {
            return CommandExecute.Error(null);
        }
        public static CommandExecute Error(string msg)
        {
            return CommandExecute.Create(CommandExecuteResult.CER_EXECUTE_ERROR, msg, null);
        }
        public static CommandExecute Succces()
        {
            return CommandExecute.Succces(null);
        }
        public static CommandExecute Succces(string msg)
        {
            return CommandExecute.Create(CommandExecuteResult.CER_EXECUTE_SUCCESS, msg, null);
        }
        public static CommandExecute Create(CommandExecuteResult result, string msg,object data)
        {
            return new CommandExecute(result, msg, data);

        }

    }
}
