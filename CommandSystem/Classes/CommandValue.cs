using CommonUtils.CommandSystem.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CommonUtils.CommandSystem.Classes
{
    public class CommandValue
    {
        public CommandValue()
        {
            this.ValueType = CommandValueType.CVT_ANY;
        }
        public CommandValueType ValueType { get; set; }
        public object Value { get; set; }
        public bool IsPreset { get; set; }

        public void Init(object v)
        {
 
            if (v is string) this.ValueType = CommandValueType.CVT_STRING;
            else if (v is int) this.ValueType = CommandValueType.CVT_INT;
            else if (v is float) this.ValueType = CommandValueType.CVT_FLOAT;
            else if (v is double) this.ValueType = CommandValueType.CVT_DOUBLE;
            else if (v is bool) this.ValueType = CommandValueType.CVT_BOOL;
            else if (v is char) this.ValueType = CommandValueType.CVT_CHAR;
            else if (v is DateTime) this.ValueType = CommandValueType.CVT_DATETIME;
            else if (v is Point) this.ValueType = CommandValueType.CVT_POINT;
            else if (v is PointF) this.ValueType = CommandValueType.CVT_POINTF;
            this.Value = v;
        }
        public override string ToString()
        {
            if (this.Value == null) return "";
            return this.Value.ToString();
        }
        public static implicit operator string(CommandValue v)
        {
            if (v != null && v.ValueType == CommandValueType.CVT_STRING)
            {
                return (string)v.Value;
            }
            return null;
        }
        public static implicit operator int(CommandValue v)
        {
            if (v != null && v.ValueType == CommandValueType.CVT_INT)
            {
                return (int)v.Value;
            }
            return 0;
        }
        public static implicit operator float(CommandValue v)
        {
            if (v != null && v.ValueType == CommandValueType.CVT_FLOAT)
            {
                return (float)v.Value;
            }
            return 0;
        }
        public static implicit operator double(CommandValue v)
        {
            if (v != null && v.ValueType == CommandValueType.CVT_DOUBLE)
            {
                return (double)v.Value;
            }
            return 0;
        }
        public static implicit operator char(CommandValue v)
        {
            if (v != null && v.ValueType == CommandValueType.CVT_CHAR)
            {
                return (char)v.Value;
            }
            return '\0';
        }
        public static implicit operator bool(CommandValue v)
        {
            if (v != null && v.ValueType == CommandValueType.CVT_BOOL)
            {
                return (bool)v.Value;
            }
            return false;
        }
        public static implicit operator DateTime(CommandValue v)
        {
            if (v != null && v.ValueType == CommandValueType.CVT_FLOAT)
            {
                return (DateTime)v.Value;
            }
            return default(DateTime);
        }
        public static implicit operator Point(CommandValue v)
        {
            if (v != null && v.ValueType == CommandValueType.CVT_POINT)
            {
                return (Point)v.Value;
            }
            return default(Point);
        }
        public static implicit operator PointF(CommandValue v)
        {
            if (v != null && v.ValueType == CommandValueType.CVT_POINTF)
            {
                return (PointF)v.Value;
            }
            return default(PointF);
        }
        public static CommandValue InitS(object v)
        {
            CommandValue cv = new CommandValue();
            cv.Init(v);
            return cv;
        }

    }
}
