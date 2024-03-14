using ARCVX.Reader;
using BCnEncoder.Encoder;
using BCnEncoder.Shared;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ARCVX.Formats
{
    public class Tex : Base<TexHeader>
    {
        public override int MAGIC { get; } = 0x54455800; // "TEX."
        public override int MAGIC_LE { get; } = 0x00584554; // ".XET"

        public Tex(FileInfo file) : base(file) { }
        public Tex(FileInfo file, Stream stream) : base(file, stream) { }

        public override TexHeader GetHeader()
        {
            if (!IsValid)
                return new TexHeader { };

            OpenReader();

            Reader.SetPosition(0);

            TexHeader header = new();

            header.Magic = Reader.ReadInt32(ByteOrder.LittleEndian);

            uint[] data = new uint[3];
            for (int i = 0; i < data.Length; i++)
                data[i] = Reader.ReadUInt32();

            header.Version = (short)(data[0] & 0xFFF);
            header.Unknown0 = (short)((data[0] >> 12) & 0xFFF);
            header.Unknown1 = (byte)((data[0] >> 28) & 0xF);
            header.AlphaFlags = (byte)((data[0] >> 32) & 0xF);

            header.MipMapCount = (byte)(data[1] & 0x3F);
            header.Width = (short)((data[1] >> 6) & 0x1FFF);
            header.Height = (short)((data[1] >> 19) & 0x1FFF);

            header.ImageCount = (byte)(data[2] & 0xFF);
            header.Format = (byte)((data[2] >> 8) & 0xFF);
            header.Unknown2 = (short)(data[2] >> 16);

            return header;
        }

        public DDSHeader GetDDSHeader() =>
            new()
            {
                Magic = 0x20534444,

                Size = 124,
                Flags = Header.MipMapCount > 0 ? 0x00021007 : 0x00001007,
                Height = Header.Height,
                Width = Header.Width,
                MipMapCount = Header.MipMapCount,

                PixelFormatSize = 32,
                PixelFormatFlags = 0x00000004,
                PixelFormatFourCC = Header.Format == 0x18 ? 0x35545844 : 0x31545844,

                Caps = 0x00001000
            };

        public long GetPixelOffset() =>
            16 + (Header.Unknown1 != 6 ? Header.MipMapCount * 4 : 0);

        public byte[] GetPixelBytes()
        {
            if (!IsValid)
                return null;

            Reader.SetPosition(GetPixelOffset());

            return Reader.ReadBytes((int)(File.Length - Reader.GetPosition()));
        }

        public MemoryStream ConvertARGBToDSS()
        {
            BcEncoder encoder = new();

            encoder.OutputOptions.GenerateMipMaps = true;
            encoder.OutputOptions.Quality = CompressionQuality.BestQuality;
            encoder.OutputOptions.Format = CompressionFormat.Bc3;
            encoder.OutputOptions.FileFormat = OutputFileFormat.Dds;

            MemoryStream stream = new();
            encoder.EncodeToStream(GetPixelBytes(), Header.Width, Header.Height, PixelFormat.Argb32, stream);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        public FileInfo Export() =>
            ExportDDS() ?? ExportARGB();

        public FileInfo ExportDDS()
        {
            try
            {
                if (Header.Unknown1 == 0x06 ||
                   (Header.Format != 0x14 &&
                    Header.Format != 0x18 &&
                    Header.Format != 0x19 &&
                    Header.Format != 0x1E))
                    throw new Exception();

                FileInfo output = new(Path.ChangeExtension(File.FullName, "dds"));
                using FileStream stream = output.OpenWrite();

                byte[] head = GetBytes(GetDDSHeader());
                byte[] data = GetPixelBytes();

                stream.Write(head, 0, head.Length);
                stream.Write(data, 0, data.Length);

                return output;
            }
            catch
            {
                return null;
            }
        }

        public FileInfo ExportARGB()
        {
            try
            {
                if (Header.Format != 0x28)
                    throw new Exception();

                FileInfo output = new(Path.ChangeExtension(File.FullName, "dds"));
                using FileStream stream = output.OpenWrite();

                ConvertARGBToDSS().CopyTo(stream);

                return output;
            }
            catch
            {
                return null;
            }
        }

        private static byte[] GetBytes<T>(T data)
            where T : struct
        {
            int size = Marshal.SizeOf(data);
            byte[] arr = new byte[size];

            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(data, ptr, false);
                Marshal.Copy(ptr, arr, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return arr;
        }
    }

    public struct TexHeader
    {
        public int Magic;

        public short Version;
        public short Unknown0;
        public byte Unknown1; // 0x02=standard,0x03=unknown,0x06=cubemaps???
        public byte AlphaFlags;

        public byte MipMapCount;
        public short Width;
        public short Height;

        public byte ImageCount;
        public byte Format; // 0x14=DXT1,0x18=DXT5,0x18=DXT1,0x1E=DXT1,0x28=ARBG8888
                            // 0x07=unknown,0x09=unknown,0x23=unknown,0x2B=unknown
        public short Unknown2;
    }

    public struct DDSHeader
    {
        public int Magic;

        public int Size;
        public int Flags;
        public int Height;
        public int Width;
        public int PitchOrLinearSize;
        public int Depth;
        public int MipMapCount;

        public int Reserved1;
        public int Reserved2;
        public int Reserved3;
        public int Reserved4;
        public int Reserved5;
        public int Reserved6;
        public int Reserved7;
        public int Reserved8;
        public int Reserved9;
        public int Reserved10;
        public int Reserved11;

        public int PixelFormatSize;
        public int PixelFormatFlags;
        public int PixelFormatFourCC;
        public int PixelFormatRGBBitCount;
        public int PixelFormatRBitMask;
        public int PixelFormatGBitMask;
        public int PixelFormatBBitMask;
        public int PixelFormatABitMask;

        public int Caps;
        public int Caps2;
        public int Caps3;
        public int Caps4;

        public int Reserved12;
    }
}