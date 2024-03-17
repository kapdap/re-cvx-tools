using ARCVX.Reader;
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

        public FileInfo SaveStream(MemoryStream stream) =>
            SaveStream(stream, File);

        public FileInfo SaveStream(MemoryStream stream, FileInfo file)
        {
            FileInfo tempFile = new(Path.Join(File.DirectoryName, "_" + Path.GetRandomFileName()));
            
            try
            {
                if (!tempFile.Directory.Exists)
                    tempFile.Directory.Create();

                using (MemoryStream verifiedStream = Hash.Helper.WriteVerification(stream))
                    using (FileStream tempStream = tempFile.OpenWrite())
                        verifiedStream.CopyTo(tempStream);

                tempFile.MoveTo(file.FullName, true);

                return file;
            }
            catch
            {
                try { tempFile.Delete(); } catch { }
                return null;
            }
        }

        public bool VerifyBlockChecksum(Span<byte> buffer, Span<byte> checksum) =>
            checksum.SequenceEqual(Hash.Helper.ComputeVerification(buffer));
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