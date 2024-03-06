using System;
using System.IO;
using System.Text;

namespace ARCVX.Reader
{
    // Adapted from https://github.com/LJW-Dev/Big-Endian-Binary-Reader/blob/main/Reader.cs
    public class EndianReader : IDisposable
    {
        public bool IsBigEndian = true;
        public BinaryReader BaseReader;

        public EndianReader(Stream stream, bool isBigEndian)
        {
            IsBigEndian = isBigEndian;
            BaseReader = new BinaryReader(stream);
        }

        public EndianReader(Stream stream) =>
            BaseReader = new BinaryReader(stream);

        public void Close() =>
            BaseReader.Close();

        public int PeekChar() =>
            BaseReader.PeekChar();

        public bool ReadBoolean() =>
            BaseReader.ReadBoolean();

        public byte ReadByte() =>
            BaseReader.ReadByte();

        public byte[] ReadBytes(int count) =>
            BaseReader.ReadBytes(count);

        public short ReadInt16(bool? isBigEndian = null)
        {
            byte[] data = BaseReader.ReadBytes(2);
            if (isBigEndian ?? IsBigEndian) Array.Reverse(data);
            return BitConverter.ToInt16(data, 0);
        }

        public ushort ReadUInt16(bool? isBigEndian = null)
        {
            byte[] data = BaseReader.ReadBytes(2);
            if (isBigEndian ?? IsBigEndian) Array.Reverse(data);
            return BitConverter.ToUInt16(data, 0);
        }

        public int ReadInt32(bool? isBigEndian = null)
        {
            byte[] data = BaseReader.ReadBytes(4);
            if (isBigEndian ?? IsBigEndian) Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }
        public uint ReadUInt32(bool? isBigEndian = null)
        {
            byte[] data = BaseReader.ReadBytes(4);
            if (isBigEndian ?? IsBigEndian) Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        public long ReadInt64(bool? isBigEndian = null)
        {
            byte[] data = BaseReader.ReadBytes(8);
            if (isBigEndian ?? IsBigEndian) Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }

        public ulong ReadUInt64(bool? isBigEndian = null)
        {
            byte[] data = BaseReader.ReadBytes(8);
            if (isBigEndian ?? IsBigEndian) Array.Reverse(data);
            return BitConverter.ToUInt64(data, 0);
        }
        public float ReadFloat(bool? isBigEndian = null)
        {
            byte[] data = BaseReader.ReadBytes(4);
            if (isBigEndian ?? IsBigEndian) Array.Reverse(data);
            return BitConverter.ToSingle(data, 0);
        }

        public double ReadDouble(bool? isBigEndian = null)
        {
            byte[] data = BaseReader.ReadBytes(8);
            if (isBigEndian ?? IsBigEndian) Array.Reverse(data);
            return BitConverter.ToDouble(data, 0);
        }

        public string ReadNullTerminatedString(int maxSize = -1)
        {
            StringBuilder builder = new();

            int size = 0;
            int str;

            while ((str = BaseReader.ReadByte()) != 0x0 && size++ != maxSize)
                builder.Append(Convert.ToChar(str));

            return builder.ToString();
        }

        public long GetPosition() =>
            BaseReader.BaseStream.Position;

        public void SetPosition(long position) =>
            BaseReader.BaseStream.Position = position;

        public void AddPosition(long length) =>
            BaseReader.BaseStream.Position += length;

        public long GetLength() =>
            BaseReader.BaseStream.Length;

        public void Dispose() => ((IDisposable)BaseReader).Dispose();
    }
}