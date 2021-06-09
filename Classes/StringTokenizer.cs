using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonUtils.Classes
{
    [Flags]
    public enum StringQuoteOption
    {
        None,
        SingleQuote,
        DoubleQuote,
        All
    }
    public class StringTokenResult
    {
        public string TokenText { get; set; }
        public string TokenKey { get; set; }
        public bool TokenFound { get; set; }
        public int TokenIndex { get; set; }
        public void Next(params string[] tokens)
        {
            var result = this.Tokenizer.Tokenize(tokens);
            this.TokenText = result.TokenText;
            this.TokenKey = result.TokenKey;
            this.TokenFound = result.TokenFound;
            this.TokenIndex = result.TokenIndex;
        }
        public StringTokenResult NextR(params string[] tokens)
        {
            return this.Tokenizer.Tokenize(tokens);
        }
        internal StringTokenizer  Tokenizer { get; set; }
    }
    public class StringTokenizer : IDisposable
    {
        public StringTokenizer()
        {

        }
        public StringTokenizer(string text)
        {
            this.Text = text;
        }
        public StringTokenizer(string text, string[] tokens)
        {
            this.Text = text;
            this.Tokens = tokens;
        }
        private string text;
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                this.ResetPosition();
            }
        }
        public bool Finish { get; private set; }
        private bool autoOrderTokens;
        public bool AutoOrderTokens
        {
            get
            {
                return autoOrderTokens;
            }
            set
            {
                autoOrderTokens = value;
                if (autoOrderTokens)
                {
                    this.Tokens = this.Tokens;
                }
            }
        }
        private string[] tokens;
        public string[] Tokens
        {
            get
            {
                return tokens;
            }
            set
            {
                this.Maxlen = 0;
                if (value == null)
                {
                    tokens = null;
                    return;
                }
                bool isordered = false;
                if (value.Length > 1)
                {
                    isordered = true;
                    if (this.AutoOrderTokens)
                    {
                        tokens = value.OrderByDescending(m => m.Length).ToArray();
                    }
                    else
                    {
                        tokens = value;
                    }

                }
                else
                {
                    isordered = true;
                    tokens = value;
                }
                if (tokens != null && tokens.Length > 0)
                {
                    if (isordered)
                        this.Maxlen = tokens.First().Length;
                    else
                        this.Maxlen = tokens.OrderByDescending(m => m.Length).First().Length;
                }

            }
        }
        public void ResetPosition()
        {
            this.Finish = false;
            this.CurrentPosition = 0;
        }
        private int currentPosition;
        public int CurrentPosition
        {
            get
            {
                return this.currentPosition;
            }
            set
            {
                this.Finish = false;
                this.currentPosition = value;
                if (this.Text != null && this.currentPosition >= this.Text.Length)
                {
                    this.Finish = true;
                }
            }
        }
        private int Maxlen
        {
            get; set;
        }
        public bool PrintSpecialCharacter { get; set; }
        public bool PrintQuote { get; set; }
        public bool AddToAll { get; set; }
        public bool AutoTrim { get; set; }
        public bool AllowSpecialChar { get; set; }
        public bool SkipEmptyValue { get; set; }
        public StringSplitOption GetSettings()
        {
            StringSplitOption splitOption = StringSplitOption.None;
            if (this.AddToAll) splitOption |= StringSplitOption.AddToAll;
            if (this.AutoTrim) splitOption |= StringSplitOption.TrimPerElement;
            if (this.PrintSpecialCharacter) splitOption |= StringSplitOption.PrintSpecialCharacter;
            if (this.PrintQuote) splitOption |= StringSplitOption.PrintQuote;
            if (this.AllowSpecialChar) splitOption |= StringSplitOption.AllowSpecialChar;
            if (this.AutoOrderTokens) splitOption |= StringSplitOption.AutoOrderTokens;
            return splitOption;
        }
        public void SetSettingsFrom(StringSplitOption stringSplitOption)
        {
            this.AddToAll = stringSplitOption.HasFlag(StringSplitOption.AddToAll);
            this.AutoTrim = stringSplitOption.HasFlag(StringSplitOption.TrimPerElement);
            this.PrintSpecialCharacter = stringSplitOption.HasFlag(StringSplitOption.PrintSpecialCharacter);
            this.PrintQuote = stringSplitOption.HasFlag(StringSplitOption.PrintQuote);
            this.AllowSpecialChar = stringSplitOption.HasFlag(StringSplitOption.AllowSpecialChar);
            this.AutoOrderTokens = stringSplitOption.HasFlag(StringSplitOption.AutoOrderTokens);

        }
        public StringQuoteOption StringQuoteOption { get; set; } = StringQuoteOption.DoubleQuote;
        public string GetRemainText()
        {
            if (this.Finish || string.IsNullOrEmpty(this.Text))
            {
                return null;
            }

            return this.Text.Substring(this.CurrentPosition);
        }
        public StringTokenResult Tokenize(StringQuoteOption stringQuoteOption, StringSplitOption stringSplitOption, params string[] tokens)
        {
            this.SetSettingsFrom(stringSplitOption);
            this.StringQuoteOption = stringQuoteOption;
            return Tokenize(tokens);
        }
        public StringTokenResult Tokenize(StringQuoteOption stringQuoteOption, params string[] tokens)
        {
            this.StringQuoteOption = stringQuoteOption;
            return Tokenize(tokens);
        }
        public StringTokenResult Tokenize(params string[] tokens)
        {
            StringTokenResult tokenResult = new StringTokenResult();
            tokenResult.Tokenizer = this;
            tokenResult.TokenIndex = -1;
            if (tokens != null && tokens.Length > 0) this.Tokens = tokens;
            if (string.IsNullOrEmpty(this.Text) || this.Tokens == null || this.Tokens.Length == 0) return tokenResult;
            if (this.CurrentPosition >= this.Text.Length)
            {
                return tokenResult;
            }
            StringBuilder currentKey = new StringBuilder();
            StringBuilder currentValue = new StringBuilder();
            bool specialchar = false;
            bool isquote = false;
            bool quoted = false;
            char quotchar = '\0';
            for (int i = this.CurrentPosition; i < this.Text.Length; i++)
            {
                char current = this.Text[i];
                if (this.AllowSpecialChar && current == '\\' && !specialchar)
                {
                    if (this.PrintSpecialCharacter)
                    {
                        currentValue.Append(current);
                    }
                    specialchar = true;
                    continue;
                }
                bool continueNext = false;
                if (this.StringQuoteOption != StringQuoteOption.None)
                {
                    if ((this.StringQuoteOption == StringQuoteOption.SingleQuote && current == '\'') || (this.StringQuoteOption == StringQuoteOption.DoubleQuote && current == '"'))
                    {
                        continueNext = true;
                    }
                    else if (this.StringQuoteOption == StringQuoteOption.All)
                    {
                        if (isquote)
                        {
                            if (current == quotchar)
                            {
                                continueNext = true;
                            }
                        }
                        else
                        {
                            if(current == '\'' || current == '"')
                            {
                                continueNext = true;
                                quotchar = current;
                            }
                        }
                    }
                }
                if (continueNext && !specialchar)
                {
                    if (isquote)
                    {
                        quoted = true;
                    }
                    isquote = !isquote;

                    if (this.PrintQuote)
                    {
                        currentValue.Append(current);
                    }


                    continue;
                }
                else
                {
                    if (!isquote)
                    {
                        
                        currentKey.Append(current);
                    }

                    if (currentKey.Length > this.Maxlen)
                    {
                        currentKey.Remove(0, 1);
                    }
           
                    if (currentKey.Length >= this.Maxlen && !isquote && !specialchar)
                    {
                        //bool next = false;
                        for (int j = 0; j < this.Tokens.Length; j++)
                        {
                            if (currentKey.ToString(0, this.Tokens[j].Length) == this.Tokens[j])
                            {
                                currentKey.Remove(0, this.Tokens[j].Length);
                                var value = "";
                                if (quoted && !this.AddToAll)
                                {
                                    value = currentValue.ToString();
                                }
                                else
                                {
                                    value = currentValue.ToString(0, currentValue.Length - (this.Maxlen - 1));
                                }
                                if (this.AutoTrim)
                                {
                                    value = value.Trim();
                                }
                                if(this.SkipEmptyValue && value.Length == 0)
                                {
                                    break;
                                }
                                currentValue.Clear();
                                currentValue.Append(currentKey.ToString());
                                this.CurrentPosition = i + 1 - (this.Maxlen - this.Tokens[j].Length);
                                tokenResult.TokenFound = true;
                                tokenResult.TokenText = value;
                                tokenResult.TokenKey = this.Tokens[j];
                                tokenResult.TokenIndex = this.CurrentPosition - (tokenResult.TokenKey.Length);
                                return tokenResult;
                            }
                        }
                        //if (next) continue;
                    }
                    if ((!quoted || AddToAll))
                    {
                        currentValue.Append(current);
                    }

                }
                if (specialchar)
                {
                    specialchar = false;
                    continue;
                }
            }
            if (currentValue.Length > 0)
            {
                string text = this.AutoTrim ? currentValue.ToString().Trim() : currentValue.ToString();
                tokenResult.TokenText = text;
            }
            this.CurrentPosition = this.text.Length;
            this.Finish = true;
            return tokenResult;
        }
        private bool disposed;
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                this.Text = null;
                this.Tokens = null;
            }
            disposed = true;
        }
    }
}
