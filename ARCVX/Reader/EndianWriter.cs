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
using System.Buffers.Binary;
using System.IO;

namespace ARCVX.Reader
{
    // Adapted from https://github.com/LJW-Dev/Big-Endian-Binary-Reader/blob/main/Reader.cs
    public class EndianWriter : IDisposable
    {
        public ByteOrder ByteOrder = ByteOrder.BigEndian;
        public BinaryWriter BaseWriter;

        public EndianWriter(Stream stream, ByteOrder order)
        {
            ByteOrder = order;
            BaseWriter = new BinaryWriter(stream);
        }

        public EndianWriter(Stream stream) =>
            BaseWriter = new BinaryWriter(stream);

        public void Close() =>
            BaseWriter.Close();

        public void Write(bool value) =>
            BaseWriter.Write(value);

        public void Write(byte value) =>
            BaseWriter.Write(value);

        public void Write(byte[] buffer) =>
            BaseWriter.Write(buffer);

        public void Write(sbyte value) =>
            BaseWriter.Write(value);

        public void Write(char ch) =>
            BaseWriter.Write(ch);

        public void Write(char[] chars) =>
            BaseWriter.Write(chars);

        public void Write(short value, ByteOrder? order = null)
        {
            if ((order ?? ByteOrder) == ByteOrder.BigEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            BaseWriter.Write(value);
        }

        public void Write(ushort value, ByteOrder? order = null)
        {
            if ((order ?? ByteOrder) == ByteOrder.BigEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            BaseWriter.Write(value);
        }

        public void Write(int value, ByteOrder? order = null)
        {
            if ((order ?? ByteOrder) == ByteOrder.BigEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            BaseWriter.Write(value);
        }

        public void Write(uint value, ByteOrder? order = null)
        {
            if ((order ?? ByteOrder) == ByteOrder.BigEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            BaseWriter.Write(value);
        }

        public void Write(long value, ByteOrder? order = null)
        {
            if ((order ?? ByteOrder) == ByteOrder.BigEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            BaseWriter.Write(value);
        }

        public void Write(ulong value, ByteOrder? order = null)
        {
            if ((order ?? ByteOrder) == ByteOrder.BigEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            BaseWriter.Write(value);
        }

        public void Write(double value, ByteOrder? order = null)
        {
            if ((order ?? ByteOrder) == ByteOrder.BigEndian)
            {
                byte[] buffer = BitConverter.GetBytes(value);
                Array.Reverse(buffer);
                BaseWriter.Write(buffer);
                return;
            }

            BaseWriter.Write(value);
        }

        public void Write(float value, ByteOrder? order = null)
        {
            if ((order ?? ByteOrder) == ByteOrder.BigEndian)
            {
                byte[] buffer = BitConverter.GetBytes(value);
                Array.Reverse(buffer);
                BaseWriter.Write(buffer);
                return;
            }

            BaseWriter.Write(value);
        }

        public void Write(Half value, ByteOrder? order = null)
        {
            if ((order ?? ByteOrder) == ByteOrder.BigEndian)
            {
                byte[] buffer = BitConverter.GetBytes(value);
                Array.Reverse(buffer);
                BaseWriter.Write(buffer);
                return;
            }

            BaseWriter.Write(value);
        }

        public void Write(string value) =>
            BaseWriter.Write(value);

        public void Write(ReadOnlySpan<byte> buffer) =>
            BaseWriter.Write(buffer);

        public void Write(ReadOnlySpan<char> chars) =>
            BaseWriter.Write(chars);

        public void Write(byte[] buffer, int index, int count) =>
            BaseWriter.Write(buffer, index, count);

        public void Write(char[] chars, int index, int count) =>
            BaseWriter.Write(chars, index, count);

        public long GetPosition() =>
            BaseWriter.BaseStream.Position;

        public void SetPosition(long position) =>
            BaseWriter.BaseStream.Position = position;

        public void AddPosition(long length) =>
            BaseWriter.BaseStream.Position += length;

        public long GetLength() =>
            BaseWriter.BaseStream.Length;

        public void Dispose() => ((IDisposable)BaseWriter).Dispose();
    }
}