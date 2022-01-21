using CommonUtils.Classes;
using CommonUtils.CommandSystem.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Utils
{
    public static class CommandUtils
    {
        public static CommandValueList ParseCommandParam(string commandParam)
        {
            var commandItems = new CommandValueList();
            StringTokenizer stringTokenizer = new StringTokenizer(commandParam);
            stringTokenizer.SkipEmptyValue = true;
            StringSplitOption option = StringSplitOption.TrimPerElement | StringSplitOption.AllowSpecialChar;
            stringTokenizer.SetSettingsFrom(option);
            while (!stringTokenizer.Finish)
            {
                CommandValueItem item = new CommandValueItem();
                stringTokenizer.StringQuoteOption = StringQuoteOption.All;
                var result = stringTokenizer.Tokenize(" ", "=");
                if (result.TokenKey == "=")
                {
                    item.Name = result.TokenText;
                    result = stringTokenizer.Tokenize(" ", "=");
                }
                if (result.TokenKey == " " || !result.TokenFound)
                {
                    item.Values.Add(result.TokenText);
                  //  item.Value = result.TokenText;
                }
                commandItems.Add(item);
            }
            return commandItems;
        }
        public static CommandParamHeader ParseSingleHeader(string header, CommandBase command)
        {
            if (string.IsNullOrEmpty(header))
            {
                return null;
            }
            CommandParamHeader parameter = new CommandParamHeader();
            parameter.Command = command;
            StringTokenizer stringTokenizer = new StringTokenizer(header);
            StringSplitOption option = StringSplitOption.TrimPerElement;
            stringTokenizer.SkipEmptyValue = true;
            while (!stringTokenizer.Finish)
            {
                parameter.Command = command;
                var result = stringTokenizer.Tokenize(StringQuoteOption.None, option, "(");
                parameter.TypeString = result.TokenText;
                if (string.IsNullOrEmpty(parameter.TypeString))
                {
                    return null;
                }
                if (result.TokenKey == "(") //has attributes
                {
                    while (result.TokenFound && result.TokenKey != ")")
                    {
                        result = stringTokenizer.Tokenize(StringQuoteOption.None, ":", ",", ")");
                        string attrName = "";
                        string attrValue = "";
                        if (result.TokenKey == ":")
                        {
                            attrName = result.TokenText.ToLowerInvariant();
                            result = stringTokenizer.Tokenize(StringQuoteOption.All, ",", ")");
                            attrValue = result.TokenText;
                        }
                        else
                        {
                            attrName = result.TokenText.ToLowerInvariant();
                        }
                        parameter.Attributes.Set(attrName, attrValue);

                    }
                    if (result.TokenKey == ")")
                    {
                        break;
                    }
                }

            }
            return parameter;
        }
        public static CommandParamHeadersList ParseCommandHeaders(string commandHeader, CommandParam command, int limit = 0)
        {
            var vs = new CommandParamHeadersList();
            int last = 0;
            if (string.IsNullOrEmpty(commandHeader)) commandHeader = "";
            int tl = commandHeader.Length;
            do
            {
                CommandParamHeaderList headers = null;
                if (last == 0)
                {
                    headers = ParseCommandHeader(commandHeader, command, out last);
                }
                else
                {
                    headers = ParseCommandHeader(commandHeader.Substring(last+ 1), command, out last);
                }
                if (headers.HasError)
                {
                    vs.ErrorMsg += headers.ErrorMsg + " ";
                }
                vs.Add(headers);
            } while (last >= 0 && last + 1 < tl && (limit <= 0 || vs.Count < limit));
            return vs;
        }
        public static CommandParamHeaderList ParseCommandHeader(string commandHeader, CommandParam command, out int last)
        {
            last = -1;
            CommandParamHeaderList commandParameters = new CommandParamHeaderList();
            if (string.IsNullOrEmpty(commandHeader))
            {
                return commandParameters;
            }
            StringTokenizer stringTokenizer = new StringTokenizer(commandHeader);
            stringTokenizer.SkipEmptyValue = true;
            StringSplitOption option = StringSplitOption.TrimPerElement | StringSplitOption.AllowSpecialChar;
            stringTokenizer.SetSettingsFrom(option);

            bool previsoptional = false;
            bool ispreset = false;
            var Container = command.Container;
            while (!stringTokenizer.Finish)
            {
                CommandParamHeader parameter = new CommandParamHeader();
                parameter.Command = command;
                var result = stringTokenizer.Tokenize(StringQuoteOption.None, option, " ");
                parameter.TypeString = result.TokenText;
                result = stringTokenizer.Tokenize(StringQuoteOption.All, option, "=", "(", ",", "|", ";");
                if (string.IsNullOrEmpty(parameter.TypeString))
                {
                    commandParameters.ErrorMsg = Container.GetText("ERR_CMD_TYPE");
                    return commandParameters;
                }
                parameter.Name = result.TokenText;

                if (result.TokenKey == "=")
                {
                    previsoptional = true;
                    parameter.HasDefaultValue = true;
                    stringTokenizer.SkipEmptyValue = false;
                    result = stringTokenizer.Tokenize(StringQuoteOption.None, option, "(", ",", "|", ";");
                    stringTokenizer.SkipEmptyValue = true;
                    parameter.DefaultValue = result.TokenText;

                }
                else if ((previsoptional && !parameter.HasDefaultValue) || (ispreset && !parameter.HasDefaultValue))
                {
                    commandParameters.ErrorMsg = Container.GetText("ERR_CMD_DEFVAL");
                    return commandParameters;
                }
                if (result.TokenKey == "(") //has attributes
                {
                    if (string.IsNullOrEmpty(parameter.Name))
                    {
                        commandParameters.ErrorMsg = Container.GetText("ERR_CMD_NAME");
                        return commandParameters;
                    }
                    while (result.TokenFound && result.TokenKey != ")")
                    {
                        result = stringTokenizer.Tokenize(StringQuoteOption.None, ":", ",", ")");
                        string attrName = "";
                        string attrValue = "";
                        if (result.TokenKey == ":")
                        {
                            attrName = result.TokenText.ToLowerInvariant();
                            result = stringTokenizer.Tokenize(StringQuoteOption.All, ",", ")");
                            attrValue = result.TokenText;
                        }
                        else
                        {
                            attrName = result.TokenText.ToLowerInvariant();
                        }
                        parameter.Attributes.Set(attrName, attrValue);

                    }
                    if (result.TokenKey == ")")
                    {
                        stringTokenizer.SkipEmptyValue = false;
                        var vr2 = stringTokenizer.Tokenize(StringQuoteOption.None, "|", ";", " ");
                        if (vr2.TokenKey == "|" || vr2.TokenKey == ";")
                        {
                            result = vr2;
                        }
                        stringTokenizer.SkipEmptyValue = true;
                    }
                }
                if (result.TokenKey == "," || result.TokenKey == ")" || result.TokenKey == "|" || result.TokenKey == ";" || !result.TokenFound)
                {

                    if(ispreset)
                    {
                        commandParameters.Presets.Add(parameter);
                    }
                    else
                    {
                        commandParameters.Add(parameter);
                    }

                    if (result.TokenKey == "|")
                    {
                        last = result.TokenIndex;
                        break;
                    }
                    else if(result.TokenKey == ";" && !ispreset)
                    {
                        commandParameters.Presets = new CommandParamHeaderList();
                        ispreset = true;
                    }
                    continue;
                }
                 
            }
            return commandParameters;
        }
    }
}
