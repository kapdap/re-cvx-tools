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
using ARCVX.Utilities;
using System;
using System.IO;

namespace ARCVX.Formats
{
    public class HFS : Base<HFSHeader>
    {
        public override int MAGIC { get; } = 0x48465300; // "HFS."
        public override int MAGIC_LE { get; } = 0x00534648; // ".SFH"

        public static int CHECK_SIZE = 0x10;
        public static int CHUNK_SIZE = 0x20000;
        public static int BLOCK_SIZE = CHUNK_SIZE - CHECK_SIZE;

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
            long position = Stream.Position;

            Stream.Seek(0, SeekOrigin.End);
            Stream.Seek(position, SeekOrigin.Begin);

            return Reader.GetLength();
        }

        public MemoryStream GetDataStream()
        {
            MemoryStream stream = new();

            Stream.Position = HEADER_SIZE;

            while (Stream.Position < RawDataLength)
            {
                int size = (int)(Stream.Position + BLOCK_SIZE > RawDataLength ? RawDataLength - Stream.Position - CHECK_SIZE : BLOCK_SIZE);

                Span<byte> buffer = Reader.ReadBytes(size);

                if (Stream.Position + CHECK_SIZE <= RawDataLength)
                {
                    Span<byte> checksum = Reader.ReadBytes(CHECK_SIZE);
                    _ = VerifyBlockChecksum(buffer, checksum);
                }

                stream.Write(buffer);
            }

            return stream;
        }

        public FileInfo SaveStream(Stream stream) =>
            SaveStream(stream, File);

        public FileInfo SaveStream(Stream stream, FileInfo file)
        {
            FileInfo outputFile = new(Path.Join(File.DirectoryName, "_" + Path.GetRandomFileName()));
            
            try
            {
                if (!outputFile.Directory.Exists)
                    outputFile.Directory.Create();

                using MemoryStream headerStream = CreateHeaderStream(stream.Length);

                if (stream.Length % 0x10 > 0)
                {
                    stream.Seek(0, SeekOrigin.End);
                    stream.Write(new byte[(int)(0x10 - stream.Length % 0x10)]);
                    stream.Seek(0, SeekOrigin.Begin);
                }

                using MemoryStream verifiedStream = Hash.Helper.WriteVerification(stream);

                using (FileStream outputStream = outputFile.OpenWrite())
                {
                    headerStream.CopyTo(outputStream);
                    verifiedStream.CopyTo(outputStream);
                }

                outputFile.Refresh();
                outputFile.MoveTo(file.FullName, true);

                return file;
            }
            catch
            {
                try { outputFile.Delete(); } catch { }
                return null;
            }
        }

        public bool VerifyBlockChecksum(Span<byte> buffer, Span<byte> checksum) =>
            checksum.SequenceEqual(Hash.Helper.ComputeVerification(buffer));

        public MemoryStream CreateHeaderStream(long length)
        {
            MemoryStream ms = new();

            ms.Write(Bytes.GetValueBytes(Header.Magic, ByteOrder.LittleEndian));
            ms.Write(Bytes.GetValueBytes(Header.Version, Reader.ByteOrder));
            ms.Write(Bytes.GetValueBytes(Header.Type, Reader.ByteOrder));
            ms.Write(Bytes.GetValueBytes((int)length, Reader.ByteOrder));
            ms.Write(Bytes.GetValueBytes(Header.Padding, Reader.ByteOrder));

            ms.Position = 0;

            return ms;
        }
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