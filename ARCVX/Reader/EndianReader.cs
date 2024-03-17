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

using System;
using System.IO;
using System.Text;

namespace ARCVX.Reader
{
    // Adapted from https://github.com/LJW-Dev/Big-Endian-Binary-Reader/blob/main/Reader.cs
    public class EndianReader : IDisposable
    {
        public ByteOrder ByteOrder = ByteOrder.BigEndian;
        public BinaryReader BaseReader;

        public EndianReader(Stream stream, ByteOrder order)
        {
            ByteOrder = order;
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

        public short ReadInt16(ByteOrder? order = null)
        {
            byte[] data = BaseReader.ReadBytes(2);
            if ((order ?? ByteOrder) == ByteOrder.BigEndian) Array.Reverse(data);
            return BitConverter.ToInt16(data, 0);
        }

        public ushort ReadUInt16(ByteOrder? order = null)
        {
            byte[] data = BaseReader.ReadBytes(2);
            if ((order ?? ByteOrder) == ByteOrder.BigEndian) Array.Reverse(data);
            return BitConverter.ToUInt16(data, 0);
        }

        public int ReadInt32(ByteOrder? order = null)
        {
            byte[] data = BaseReader.ReadBytes(4);
            if ((order ?? ByteOrder) == ByteOrder.BigEndian) Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }
        public uint ReadUInt32(ByteOrder? order = null)
        {
            byte[] data = BaseReader.ReadBytes(4);
            if ((order ?? ByteOrder) == ByteOrder.BigEndian) Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        public long ReadInt64(ByteOrder? order = null)
        {
            byte[] data = BaseReader.ReadBytes(8);
            if ((order ?? ByteOrder) == ByteOrder.BigEndian) Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }

        public ulong ReadUInt64(ByteOrder? order = null)
        {
            byte[] data = BaseReader.ReadBytes(8);
            if ((order ?? ByteOrder) == ByteOrder.BigEndian) Array.Reverse(data);
            return BitConverter.ToUInt64(data, 0);
        }
        public float ReadFloat(ByteOrder? order = null)
        {
            byte[] data = BaseReader.ReadBytes(4);
            if ((order ?? ByteOrder) == ByteOrder.BigEndian) Array.Reverse(data);
            return BitConverter.ToSingle(data, 0);
        }

        public double ReadDouble(ByteOrder? order = null)
        {
            byte[] data = BaseReader.ReadBytes(8);
            if ((order ?? ByteOrder) == ByteOrder.BigEndian) Array.Reverse(data);
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

    public enum ByteOrder
    {
        LittleEndian,
        BigEndian
    }
}