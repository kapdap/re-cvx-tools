using System;
using System.Globalization;
using System.Text;

namespace RDXplorer.Models.RDX
{
    public class DataEntryModel<T> : IValueEntryModel<T>
         where T : struct
    {
        public IntPtr Offset { get; private set; }
        public long Size { get => Data != null ? Data.LongLength : 0; }
        public byte[] Data { get; private set; }
        public T Value { get; private set; }
        public string Text { get; private set; }

        public virtual bool IsPointer { get; set; } = false;
        public virtual bool IsText { get; set; } = false;

        public void SetValue(byte[] data) =>
            SetValue(IntPtr.Zero, data);

        public void SetValue(long pointer, byte[] data) =>
            SetValue((IntPtr)pointer, data);

        public void SetValue(IntPtr pointer, byte[] data)
        {
            Data = data;
            Offset = pointer;
            Value = (T)Utilities.GetValueType(data, typeof(T));
            Text = Encoding.ASCII.GetString(Data);
        }

        public override string ToString() =>
            IsText ? Text : Value.ToString();

        public string ToString(string format)
        {
            if (IsText)
                return Text;

            if (string.IsNullOrEmpty(format))
                format = "G";

            Type type = typeof(T);

            if (type == typeof(bool))
                return Convert.ToBoolean(Value).ToString();

            if (type == typeof(byte))
                return Convert.ToByte(Value).ToString(format, NumberFormatInfo.CurrentInfo);

            if (type == typeof(decimal))
                return Convert.ToDecimal(Value).ToString(format, NumberFormatInfo.CurrentInfo);

            if (type == typeof(double))
                return Convert.ToDouble(Value).ToString(format, NumberFormatInfo.CurrentInfo);

            if (type == typeof(short))
                return Convert.ToInt16(Value).ToString(format, NumberFormatInfo.CurrentInfo);

            if (type == typeof(int))
                return Convert.ToInt32(Value).ToString(format, NumberFormatInfo.CurrentInfo);

            if (type == typeof(long))
                return Convert.ToInt64(Value).ToString(format, NumberFormatInfo.CurrentInfo);

            if (type == typeof(float))
                return Convert.ToSingle(Value).ToString(format, NumberFormatInfo.CurrentInfo);

            if (type == typeof(ushort))
                return Convert.ToUInt16(Value).ToString(format, NumberFormatInfo.CurrentInfo);

            if (type == typeof(uint))
                return Convert.ToUInt32(Value).ToString(format, NumberFormatInfo.CurrentInfo);

            if (type == typeof(ulong))
                return Convert.ToUInt64(Value).ToString(format, NumberFormatInfo.CurrentInfo);

            return Value.ToString();
        }
    }
}