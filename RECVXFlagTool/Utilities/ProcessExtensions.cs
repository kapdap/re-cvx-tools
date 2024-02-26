/*
 * The MIT License (MIT)
 *
 * Modified: Copyright(c) 2020 - 2020 Kapdap
 * Original: Copyright(c) 2013 - 2020 Christopher Serr and Sergey Papushin
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
 * https://github.com/LiveSplit/LiveSplit/blob/master/LiveSplit/LiveSplit.Core/ComponentUtil/ProcessExtensions.cs
 */

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace RECVXFlagTool.Utilities
{
    public enum StringEnumType
    {
        AutoDetect,
        ASCII,
        UTF8,
        UTF16
    }

    public static class ExtensionMethods
    {
        public static bool Is64Bit(this Process process)
        {
            _ = NativeWrappers.IsWow64Process(process.Handle, out bool procWow64);
            return Environment.Is64BitOperatingSystem && !procWow64;
        }

        public static bool IsRunning(this Process process)
        {
            int exitCode = 0;
            return NativeWrappers.GetExitCodeProcess(process.Handle, ref exitCode) && exitCode == 259;
        }

        public static int ExitCode(this Process process)
        {
            int exitCode = 0;
            _ = NativeWrappers.GetExitCodeProcess(process.Handle, ref exitCode);
            return exitCode;
        }

        public static T ReadValue<T>(this Process process, IntPtr addr, bool swap = false, T default_ = default)
            where T : struct
        {

            if (!process.ReadValue(addr, out T value, swap))
                value = default_;

            return value;
        }

        public static bool ReadValue<T>(this Process process, IntPtr addr, out T value, bool swap = false)
            where T : struct
        {

            object val;
            value = default;

            Type type = typeof(T);
            type = type.IsEnum ? Enum.GetUnderlyingType(type) : type;

            int size = type == typeof(bool) ? 1 : Marshal.SizeOf(type);

            if (!ReadBytes(process, addr, size, out byte[] bytes, swap))
                return false;

            val = ResolveToType(bytes, type);
            value = (T)val;

            return true;
        }

        public static byte[] ReadBytes(this Process process, IntPtr addr, int size, bool swap = false) =>
            !process.ReadBytes(addr, size, out byte[] bytes, swap) ? (new byte[size]) : bytes;

        public static bool ReadBytes(this Process process, IntPtr addr, int size, out byte[] value, bool swap = false)
        {
            byte[] bytes = new byte[size];
            value = null;
            if (!NativeWrappers.ReadProcessMemory(process.Handle, addr, bytes, size, out _))
                return false;

            if (swap)
                Array.Reverse(bytes);

            value = bytes;

            return true;
        }

        public static string ReadString(this Process process, IntPtr addr, int size, bool swap = false, string default_ = null) =>
            !process.ReadString(addr, size, out string str, swap) ? default_ : str;

        public static string ReadString(this Process process, IntPtr addr, StringEnumType type, int size, bool swap = false, string default_ = null) =>
            !process.ReadString(addr, type, size, out string str, swap) ? default_ : str;

        public static bool ReadString(this Process process, IntPtr addr, int size, out string value, bool swap = false) =>
            ReadString(process, addr, StringEnumType.AutoDetect, size, out value, swap);

        public static bool ReadString(this Process process, IntPtr addr, StringEnumType type, int size, out string value, bool swap = false)
        {
            byte[] bytes = new byte[size];
            value = null;

            if (!NativeWrappers.ReadProcessMemory(process.Handle, addr, bytes, size, out IntPtr read))
                return false;

            if (swap)
                Array.Reverse(bytes);

            value = type == StringEnumType.AutoDetect
                ? read.ToInt64() >= 2 && bytes[1] == '\x0' ? Encoding.Unicode.GetString(bytes) : Encoding.UTF8.GetString(bytes)
                : type == StringEnumType.UTF8
                ? Encoding.UTF8.GetString(bytes)
                : type == StringEnumType.UTF16 ? Encoding.Unicode.GetString(bytes) : Encoding.ASCII.GetString(bytes);

            return true;
        }

        private static object ResolveToType(byte[] bytes, Type type)
        {
            object val;

            if (type == typeof(int))
                val = BitConverter.ToInt32(bytes, 0);
            else if (type == typeof(uint))
                val = BitConverter.ToUInt32(bytes, 0);
            else if (type == typeof(float))
                val = BitConverter.ToSingle(bytes, 0);
            else if (type == typeof(double))
                val = BitConverter.ToDouble(bytes, 0);
            else if (type == typeof(byte))
                val = bytes[0];
            else if (type == typeof(bool))
                val = bytes == null ? false : (object)(bytes[0] != 0);
            else if (type == typeof(short))
                val = BitConverter.ToInt16(bytes, 0);
            else // probably a struct
            {
                GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

                try { val = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), type); }
                finally { handle.Free(); }
            }

            return val;
        }
    }
}