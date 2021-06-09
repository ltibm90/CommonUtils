using CommonUtils.CommandSystem.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public class CommandItemContainer
    {
        public int HeadersIndex { get; set; }
        public CommandValue this[int index]
        {
            get
            {
                if (index < 0 || index >= this.CommandItems.Count) return null;
                return this.CommandItems[index].Value;
            }
        }
        public CommandValue this[string key]
        {
            get
            {
                var item = this.CommandItems[key];
                if (item == null && this.PresetsItems != null)
                {
                    item = this.PresetsItems[key];
                    item.IsPreset = true;
                }
                return item;
            }
        }
        public bool HasError
        {
            get
            {

                return !string.IsNullOrEmpty(this.ErrorMsg);
            }
        }
        public CommandExecuteData ExecuteData { get; set; }
        public string ErrorMsg { get; set; }
        public CommandItemList CommandItems { get; set; }
        public CommandItemList PresetsItems { get; set; }
        public CommandParam Command { get; set; }
     
        internal bool ConvertAndValidate(CommandItemList cmditems = null)
        {
            if (this.ErrorMsg == null) this.ErrorMsg = ""; 
            if (cmditems == null) cmditems = this.CommandItems;
            for (int i = 0; i < cmditems.Count; i++)
            {
                var item = cmditems[i];
                CommandType itemType = null;
                if (item.CommandHeader.CommandType != null) itemType = item.CommandHeader.CommandType.Invoke();
                var result = itemType.Convert(item, this.ExecuteData);
                if (!result.Success)
                {
                    this.ErrorMsg += result.ErrorMessage + ", ";
                    if (string.IsNullOrEmpty(this.ErrorMsg))
                    {
                        this.ErrorMsg = "error!";
                    }
                    return false;
                }
                else
                {

                    CustomValidateContent content = null;
                    if (this.Command.Container.OnConvert != null)
                    {
                        content = new CustomValidateContent();
                        content.Header = item.CommandHeader;
                        content.Value = result.Value;
                        content.ExecuteData = this.ExecuteData;
                        content.ParamIndex = i;
                        var cr = this.Command.Container.OnConvert(content);
                        if (!cr.Success)
                        {
                            this.ErrorMsg += cr.Message + ", ";
                            if (string.IsNullOrEmpty(this.ErrorMsg))
                            {
                                this.ErrorMsg = "error!";
                            }
                            return false;
                        }
                    }

                    if (this.Command.CustomConvert != null)
                    {
                        if(content == null)
                        {
                            content.Header = item.CommandHeader;
                            content.Value = result.Value;

                            content.ExecuteData = this.ExecuteData;
                        }

                        content.ParamIndex = i;
                        var customResult = this.Command.CustomConvert(content);
                        if(customResult != null)
                        {
                            if(!customResult.Success)
                            {
                                this.ErrorMsg += customResult.Message + ", ";
                                if (string.IsNullOrEmpty(this.ErrorMsg))
                                {
                                    this.ErrorMsg = "error!";
                                }
                                return false;
                            }
                        }
                    }
                    item.Value = result.Value;
                }
            }
            return true;
        }
    }
}
