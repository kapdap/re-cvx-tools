using System;

namespace RDXplorer
{
    public class Utilities
    {
        public static object GetValueType(byte[] bytes, Type type)
        {
            if (bytes == null || bytes.Length <= 0)
                return new();

            if (type == typeof(bool))
                return bytes[0] != 0;

            if (type == typeof(byte))
                return bytes[0];

            if (type == typeof(double))
                return BitConverter.ToDouble(bytes, 0);

            if (type == typeof(short))
                return BitConverter.ToInt16(bytes, 0);

            if (type == typeof(int))
                return BitConverter.ToInt32(bytes, 0);

            if (type == typeof(long))
                return BitConverter.ToInt64(bytes, 0);

            if (type == typeof(float))
                return BitConverter.ToSingle(bytes, 0);

            if (type == typeof(ushort))
                return BitConverter.ToUInt16(bytes, 0);

            if (type == typeof(uint))
                return BitConverter.ToUInt32(bytes, 0);

            if (type == typeof(ulong))
                return BitConverter.ToUInt64(bytes, 0);

            return new();
        }
    }
}
