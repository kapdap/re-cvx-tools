// SPDX-FileCopyrightText: 2024 Kapdap <kapdap@pm.me>
//
// SPDX-License-Identifier: MIT
/*  ARCVX
 *  
 *  Copyright 2024 Kapdap <kapdap@pm.me>
 *
 *  Use of this source code is governed by an MIT-style
 *  license that can be found in the LICENSE file or at
 *  https://opensource.org/licenses/MIT.
 */

using ARCVX.Reader;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ARCVX.Utilities
{
    public static class Bytes
    {
        public static Span<byte> GetStringBytes(string value, int size = 0)
        {
            Span<byte> bytes = Encoding.ASCII.GetBytes(value);
            return size > 0 ? RightPad(bytes, 0, size) : bytes;
        }

        public static Span<byte> GetValueBytes<T>(T value, ByteOrder order = ByteOrder.BigEndian)
            where T : IConvertible
        {
            byte[] bytes;

            if (typeof(T) == typeof(char))
                bytes = BitConverter.GetBytes(Convert.ToChar(value));
            else if (typeof(T) == typeof(bool))
                bytes = BitConverter.GetBytes(Convert.ToBoolean(value));
            else if (typeof(T) == typeof(double))
                bytes = BitConverter.GetBytes(Convert.ToDouble(value));
            else if (typeof(T) == typeof(short))
                bytes = BitConverter.GetBytes(Convert.ToInt16(value));
            else if (typeof(T) == typeof(int))
                bytes = BitConverter.GetBytes(Convert.ToInt32(value));
            else if (typeof(T) == typeof(long))
                bytes = BitConverter.GetBytes(Convert.ToInt64(value));
            else if (typeof(T) == typeof(float))
                bytes = BitConverter.GetBytes(Convert.ToSingle(value));
            else if (typeof(T) == typeof(ushort))
                bytes = BitConverter.GetBytes(Convert.ToUInt16(value));
            else if (typeof(T) == typeof(uint))
                bytes = BitConverter.GetBytes(Convert.ToUInt32(value));
            else if (typeof(T) == typeof(ulong))
                bytes = BitConverter.GetBytes(Convert.ToUInt64(value));
            else
                bytes = [Convert.ToByte(value)];

            if (order == ByteOrder.BigEndian)
                Array.Reverse(bytes);

            return bytes;
        }

        public static Span<byte> GetStructBytes<T>(T data)
            where T : struct
        {
            int size = Marshal.SizeOf(data);
            byte[] buffer = new byte[size];

            nint pointer = 0;

            try
            {
                pointer = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(data, pointer, false);
                Marshal.Copy(pointer, buffer, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(pointer);
            }

            return buffer;
        }

        public static Span<byte> RightPad(Span<byte> input, byte value, int length)
        {
            Span<byte> buffer = Enumerable.Repeat(value, length).ToArray();

            for (int i = 0; i < input.Length; i++)
                buffer[i] = input[i];

            return buffer;
        }
    }
}