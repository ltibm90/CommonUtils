using CommonUtils.CommandSystem.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public class CommandParam : CommandBase
    {
        public CommandParam()
        {
            this.AcceptMoreParameters = true;
        }
        public override CommandBaseType BaseType
        {
            get
            {
                return CommandBaseType.CT_COMMAND;
            }
        }
        private CommandParamHeader defaultHeader;
        public CommandParamHeader DefaultHeader
        {
            get
            {
                if (this.defaultHeader == null)
                {
                    this.defaultHeader = new CommandParamHeader();
                    this.defaultHeader.Command = this;
                    this.defaultHeader.TypeString = "string";
                }
                return this.defaultHeader;
            }
            set
            {
                this.defaultHeader = value;
            }
        }
        public int MinParam { get; set; }
        public int MaxParam { get; set; }
        public CommandParamHeadersList Headers { get; set; }
        public Func<CommandItemContainer, CommandExecute> Target { get; set; }
        public bool AcceptMoreParameters { get; set; }
        public CommandItemContainer BindCommand(CommandParamHeaderList paramHeader, CommandValueList cmdItems)
        {

            var container = new CommandItemContainer();
            container.Command = this;
            if (paramHeader.Count - paramHeader.OptionalCount > cmdItems.Count)
            {
                container.ErrorMsg = this.Container.GetText("CMD_USAGE", this.Name + paramHeader.GetUsageMesage());
                return container;
            }
            int count = paramHeader.Count;
            int iremoved = 0;
            if (this.AcceptMoreParameters && cmdItems.Count - paramHeader.CellCount > count)
            {
                iremoved = (cmdItems.Count - paramHeader.CellCount) - count;
                count = cmdItems.Count - paramHeader.CellCount;
            }
            var binds = new CommandItemList(count);
            container.CommandItems = binds;
            int curIndex = 0;
            int passed = 0;
            int offset = 0;
            int entered = 0;
            for (int i = 0; i < cmdItems.Count; i++)
            {

                var item = cmdItems[i];
                if (item == null || item.IsEmpty) continue;
                if (i - passed >= paramHeader.Count && !item.HasName)
                {
                    if (this.AcceptMoreParameters)
                    {
                        var bitem = new CommandItem();
                        bitem.CommandHeader = this.DefaultHeader;
                        bitem.CommandValue = item;
                        binds[i - passed] = bitem;
                        continue;
                    }
                    break;

                }
                if (item.HasName)
                {
                    curIndex = paramHeader.GetIdByName(item.Name);
                    if (curIndex == -1)
                    {
                        container.ErrorMsg = this.Container.GetText("ERR_CMD_INVALID", item.Name);
                        return container;
                    }
                    //i = curIndex;
                    offset = curIndex - i;
                }
                else
                {
                    curIndex = i - passed + offset;
                }
                if (binds[curIndex] != null)
                {
                    container.ErrorMsg = this.Container.GetText("ERR_CMD_MULTI_VAL");
                    return container;
                }
                var header = paramHeader[curIndex];
                var bindItem = new CommandItem();
                bindItem.CommandHeader = header;
                bindItem.CommandValue = item;
                binds[curIndex] = bindItem;
                entered++;
                if (header.Attributes.Cell > 1)
                {
                    int realcell = header.Attributes.GetCellFor(cmdItems, i);
                    if (realcell > header.Attributes.Cell) realcell = header.Attributes.Cell;
                    if (realcell < 1) realcell = 1;
                    if(iremoved > 0 && realcell < header.Attributes.Cell)
                    {
                        int ftotal = header.Attributes.Cell - realcell;
                        if (ftotal > iremoved) ftotal = iremoved;
                        for (int j = 0; j < ftotal; j++)
                        {
                            binds.Add(null);
                            iremoved--;
                        }
                    }
                    if (realcell == 1) continue;
                    if (curIndex + realcell - 1 >= cmdItems.Count)
                    {
                        container.ErrorMsg = this.Container.GetText("ERR_CMD_PARAM");
                        return container;
                    }
                    for (int j = 1; j < realcell; j++)
                    {
                        var cellitem = cmdItems[i + j];
                        if(cellitem.HasName)
                        {
                            container.ErrorMsg = this.Container.GetText("ERR_CMD_PARAM");
                            return container;
                        }
                        item.Values.Add(cellitem.Value);
                    }
                    i += realcell - 1;
                    passed += realcell - 1;
                    //i = curIndex + header.Attributes.Cell - 1;
                    continue;
                }
            }
            if(this.MinParam > 0 && entered < this.MinParam)
            {
                container.ErrorMsg = this.Container.GetText("ERR_CMD_PARAM_MIN", this.MinParam);
                return container;
            }
            if(this.MaxParam > 0 && entered > this.MaxParam)
            {
                container.ErrorMsg = this.Container.GetText("ERR_CMD_PARAM_MAX", this.MaxParam);
                return container;
            }
            //Search for not entered commands
            for (int i = 0; i < binds.Count; i++)
            {
                if (binds[i] != null) continue;
                var target = paramHeader[i];
                if (!target.HasDefaultValue)
                {
                    container.ErrorMsg = this.Container.GetText("ERR_CMD_PARAM_REQ", target.Name);
                    return container;
                }
                var bindItem = new CommandItem();
                bindItem.CommandHeader = target;
                bindItem.CommandValue = new CommandValueItem();
                bindItem.CommandValue.Values.Add(target.DefaultValue);
                binds[i] = bindItem;
            }

            //For preset items.
            if(paramHeader.Presets != null)
            {
                container.PresetsItems = new CommandItemList();
                for (int i = 0; i < paramHeader.Presets.Count ; i++)
                {
                    var preset = paramHeader.Presets[i];
                    var bindItem = new CommandItem();
                    bindItem.CommandHeader = preset;
                    bindItem.CommandValue = new CommandValueItem();
                    bindItem.CommandValue.Values.Add(preset.DefaultValue);
                    container.PresetsItems.Add(bindItem);
                }
            }


            return container;
        }
        public override CommandExecute Execute(string commandParam, CommandExecuteData data)
        {
            lock (this)
            {
                var executeResult = new CommandExecute();
                CommandItemContainer bindContainer = null;
                if (this.Headers.Count == 0 && !this.AcceptMoreParameters)
                {
                    bindContainer = new CommandItemContainer();
                }
                else
                {
                    var cmdItems = CommandUtils.ParseCommandParam(commandParam);
                    for (int i = 0; i < this.Headers.Count; i++)
                    {
                        bindContainer = this.BindCommand(this.Headers[i], cmdItems);
                        if (bindContainer.HasError)
                        {
                            continue;
                        }
                        bindContainer.HeadersIndex = i;
                        break;
                    }
                    if(bindContainer.HasError)
                    {
                        executeResult.Message = bindContainer.ErrorMsg;
                        return executeResult;
                    }
                    bindContainer.ExecuteData = data;
                    bindContainer.ConvertAndValidate(bindContainer.CommandItems);
                    if(!bindContainer.HasError && bindContainer.PresetsItems != null) bindContainer.ConvertAndValidate(bindContainer.PresetsItems);
                    if (bindContainer.HasError)
                    {
                        executeResult.Message = bindContainer.ErrorMsg;
                        return executeResult;
                    }

                }
                CommandExecutedEventArgs execEv = new CommandExecutedEventArgs(this, bindContainer);
                execEv.ExecuteData = data;
                this.Container.OnCommandExecutedPre(execEv);
                if (execEv.Cancel)
                {
                    executeResult.Message = this.Container.GetText("ERR_CMD_CANCELLED");
                    return executeResult;
                }
                else if (this.Target != null)
                {
                    executeResult =  this.Target.Invoke(bindContainer);
                 
                }
                this.Container.OnCommandExecutedPost(execEv);
                return executeResult;
            }
        }

        public CommandParam Then(Action<CommandParam> action)
        {
            action(this);
            return this;
        }
    }
}
