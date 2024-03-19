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

using ARCVX.Hash;
using ARCVX.Reader;
using System;
using System.IO;

namespace ARCVX.Formats
{
    public class HFS : Base<HFSHeader>
    {
        public override int MAGIC { get; } = 0x48465300; // "HFS."
        public override int MAGIC_LE { get; } = 0x00534648; // ".SFH"

        public static int ALIGN_SIZE { get; } = 0x10;
        public static int CHECK_SIZE { get; } = 0x10;
        public static int CHUNK_SIZE { get; } = 0x20000;
        public static int BLOCK_SIZE { get; } = CHUNK_SIZE - CHECK_SIZE;

        private long? _dataLength;
        public long DataLength
        {
            get
            {
                _dataLength ??= RawDataLength - (RawDataLength / CHUNK_SIZE * CHECK_SIZE) - HEADER_SIZE;
                return (long)_dataLength;
            }
        }

        private long? _rawDataLength;
        public long RawDataLength
        {
            get
            {
                _rawDataLength ??= GetRawDataLength();
                return (long)_rawDataLength;
            }
        }

        public HFS(FileInfo file) : base(file) { }
        public HFS(FileInfo file, Stream stream) : base(file, stream) { }

        public override HFSHeader GetHeader()
        {
            OpenReader();

            Reader.SetPosition(0);

            return IsValid ? new HFSHeader
            {
                Magic = Reader.ReadInt32(ByteOrder.LittleEndian),
                Version = Reader.ReadInt16(),
                Type = Reader.ReadInt16(),
                Size = Reader.ReadInt32(),
                Padding = Reader.ReadInt32(),
            } : new();
        }

        public long GetRawDataLength()
        {
            OpenReader();

            long position = Stream.Position;

            Stream.Seek(0, SeekOrigin.End);
            Stream.Seek(position, SeekOrigin.Begin);

            return Stream.Length;
        }

        public MemoryStream GetDataStream()
        {
            OpenReader();

            MemoryStream stream = new();

            Stream.Position = HEADER_SIZE;

            while (Stream.Position < RawDataLength)
            {
                int size = (int)(Stream.Position + BLOCK_SIZE > RawDataLength ? RawDataLength - Stream.Position - CHECK_SIZE : BLOCK_SIZE);

                Span<byte> buffer = new byte[size];

                Stream.Read(buffer);

                if (Stream.Position + CHECK_SIZE <= RawDataLength)
                {
                    Span<byte> checksum = new byte[CHECK_SIZE];

                    Stream.Read(checksum);

                    _ = VerifyBlockChecksum(buffer, checksum);
                }

                stream.Write(buffer);
            }

            stream.Position = 0;

            return stream;
        }

        public HFS SaveStream(Stream stream) =>
            SaveStream(stream, File);

        public HFS SaveStream(Stream stream, FileInfo file)
        {
            OpenReader();

            FileInfo outputFile = new(Path.Join(File.DirectoryName, "_" + Path.GetRandomFileName()));

            try
            {
                if (!outputFile.Directory.Exists)
                    outputFile.Directory.Create();

                using (MemoryStream headerStream = CreateHeaderStream(stream.Length))
                {
                    if (stream.Length % ALIGN_SIZE > 0)
                    {
                        stream.Position = stream.Length;
                        stream.Write(new byte[(int)(ALIGN_SIZE - (stream.Length % ALIGN_SIZE))]);
                        stream.Position = 0;
                    }

                    using MemoryStream verifyStream = Helper.WriteVerification(stream);
                    using FileStream outputStream = outputFile.OpenWrite();

                    headerStream.CopyTo(outputStream);
                    verifyStream.CopyTo(outputStream);
                }

                if (file.FullName == File.FullName)
                    CloseReader();

                if (!file.Directory.Exists)
                    file.Directory.Create();

                outputFile.Refresh();
                outputFile.MoveTo(file.FullName, true);

                return new(outputFile);
            }
            catch
            {
                try { outputFile.Delete(); } catch { }

                throw;
            }
        }

        public MemoryStream CreateHeaderStream(long length)
        {
            MemoryStream stream = new();
            EndianWriter writer = new(stream, ByteOrder);

            try
            {
                writer.Write(Header.Magic, ByteOrder.LittleEndian);
                writer.Write(Header.Version);
                writer.Write(Header.Type);
                writer.Write((int)length);
                writer.Write(Header.Padding);
            }
            catch
            {
                writer.Write(MAGIC, ByteOrder);
                writer.Write(1);
                writer.Write(1);
                writer.Write((int)length);
                writer.Write(0);
            }

            writer.SetPosition(0);

            return stream;
        }

        public bool VerifyBlockChecksum(Span<byte> buffer, Span<byte> checksum) =>
            checksum.SequenceEqual(Helper.ComputeVerification(buffer));
    }

    public struct HFSHeader
    {
        public int Magic;
        public short Version;
        public short Type;
        public int Size;
        public int Padding;
    }
}