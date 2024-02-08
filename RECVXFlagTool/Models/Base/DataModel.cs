using RECVXFlagTool.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RECVXFlagTool.Models.Base
{
    public class DataModel<T> : BaseNotifyModel where T : struct
    {
        private T _value;
        public T Value
        {
            get => _value;
            private set => SetField(ref _value, value);
        }

        private byte[] _data;
        public byte[] Data
        {
            get => _data;
            private set => SetField(ref _data, value, "Size");
        }

        private string _text;
        public string Text
        {
            get => _text;
            private set => SetField(ref _text, value);
        }

        private IntPtr _pointer;
        public IntPtr Pointer
        {
            get => _pointer;
            private set => SetField(ref _pointer, value);
        }

        private bool _isPointer = false;
        public bool IsPointer
        {
            get => _isPointer;
            private set => SetField(ref _isPointer, value);
        }

        public long Size => Data != null ? Data.LongLength : 0;
        public Type DataType { get; } = typeof(T);

        public DataModel() { }

        public DataModel(bool isPointer) => 
            IsPointer = isPointer;

        public void SetValue(T value) => 
            SetValue(value, IntPtr.Zero);

        public void SetValue(Process process, IntPtr pointer, bool swap = false, T default_ = default) => 
            SetValue(process.ReadValue(pointer, swap, default_), pointer);

        public void SetValue(T value, IntPtr pointer)
        {
            Value = value;
            Pointer = pointer;
            Data = GetBytes(Value);
            Text = Encoding.ASCII.GetString(Data);
        }

        // TODO: Move to utility class
        private static byte[] GetBytes(T value)
        {
            Type type = typeof(T);

            if (type == typeof(bool))
                return BitConverter.GetBytes(Convert.ToBoolean(value));

            if (type == typeof(byte))
                return new byte[] { Convert.ToByte(value) };

            if (type == typeof(char))
                return BitConverter.GetBytes(Convert.ToChar(value));

            if (type == typeof(decimal))
                return GetDecimalBytes(Convert.ToDecimal(value));

            if (type == typeof(double))
                return BitConverter.GetBytes(Convert.ToDouble(value));

            if (type == typeof(short))
                return BitConverter.GetBytes(Convert.ToInt16(value));

            if (type == typeof(int))
                return BitConverter.GetBytes(Convert.ToInt32(value));

            if (type == typeof(long))
                return BitConverter.GetBytes(Convert.ToInt64(value));

            if (type == typeof(float))
                return BitConverter.GetBytes(Convert.ToSingle(value));

            if (type == typeof(ushort))
                return BitConverter.GetBytes(Convert.ToInt16(value));

            if (type == typeof(uint))
                return BitConverter.GetBytes(Convert.ToUInt32(value));

            if (type == typeof(ulong))
                return BitConverter.GetBytes(Convert.ToUInt64(value));

            if (type == typeof(string))
                return Encoding.ASCII.GetBytes((string)Convert.ChangeType(value, type));

            return new byte[] { };
        }

        // TODO: move to utility class
        private static byte[] GetDecimalBytes(decimal dec)
        {
            int[] bits = decimal.GetBits(dec);
            List<byte> bytes = new();

            foreach (int i in bits)
                bytes.AddRange(BitConverter.GetBytes(i));

            return bytes.ToArray();
        }
    }
}