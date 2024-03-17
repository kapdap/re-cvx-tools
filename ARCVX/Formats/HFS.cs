using ARCVX.Reader;
using System;
using System.IO;

namespace ARCVX.Formats
{
    public class HFS : Base<HFSHeader>
    {
        public override int MAGIC { get; } = 0x48465300; // "HFS."
        public override int MAGIC_LE { get; } = 0x00534648; // ".SFH"

        public const int CHECK_SIZE = 0x10;
        public const int CHUNK_SIZE = 0x20000;
        public const int BLOCK_SIZE = CHUNK_SIZE - CHECK_SIZE;

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

            Reader.SetPosition(HEADER_SIZE);

            long read = 0;
            while (read < DataLength)
            {
                ReadOnlySpan<byte> data;
                int size = (int)(read + BLOCK_SIZE > RawDataLength ? RawDataLength - Reader.GetPosition() : BLOCK_SIZE);

                data = Reader.ReadBytes(size);
                read += size;

                if (read + CHECK_SIZE <= RawDataLength)
                    _ = Reader.ReadBytes(CHECK_SIZE);

                stream.Write(data);
            }

            return stream;
        }

        public FileInfo Save(MemoryStream stream) =>
            throw new NotImplementedException("Save has not been implemented");

        public ReadOnlySpan<byte> GenerateBlockChecksum(ReadOnlySpan<byte> data) =>
            throw new NotImplementedException("GenerateBlockChecksum has not been implemented");

        public bool VerifyBlockChecksum(ReadOnlySpan<byte> data, ReadOnlySpan<byte> checksum) =>
            true;
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