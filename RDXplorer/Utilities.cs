﻿using System;
using System.IO;
using System.Security.Cryptography;

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

        public static string GetFileMD5(string path) =>
            GetFileMD5((FileInfo)new(path));

        public static string GetFileMD5(FileInfo file)
        {
            using Stream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return GetFileMD5(stream);
        }

        public static string GetFileMD5(Stream stream)
        {
            using MD5 crypt = MD5.Create();
            string hash = BitConverter.ToString(crypt.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
            stream.Seek(0, SeekOrigin.Begin);
            return hash;
        }

        public static ushort SwapBytes(ushort value) =>
            (ushort)((ushort)((value & 0xff) << 8) | ((value >> 8) & 0xff));
    }
}
