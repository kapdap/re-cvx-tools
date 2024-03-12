using ARCVX.Reader;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ARCVX.Formats
{
    public class Tex : IDisposable
    {
        public const int MAGIC_TEX = 0x54455800; // "TEX."
        public const int MAGIC_TEX_LE = 0x00584554; // ".XET"

        public FileInfo File { get; }
        public FileStream Stream { get; private set; }
        public EndianReader Reader { get; private set; }

        private int? _magic;
        public int Magic
        {
            get
            {
                if (_magic == null)
                    _magic = GetMagic();
                return (int)_magic;
            }
        }

        private TexHeader? _header;
        public TexHeader Header
        { 
            get
            {
                if (_header == null && IsValid)
                    _header = GetTexHeader();
                return _header ?? new TexHeader();
            }
        }

        private bool? _isValid = null;
        public bool IsValid
        {
            get
            {
                if (_isValid == null)
                {
                    _isValid = File.Exists && File.Length > 16;

                    if ((bool)_isValid)
                        _isValid = Magic == MAGIC_TEX || Magic == MAGIC_TEX_LE;
                }

                return (bool)_isValid;
            }
        }

        public Tex(FileInfo file) =>
            File = file;

        public int GetMagic()
        {
            OpenReader();

            long position = Reader.GetPosition();
            Reader.SetPosition(0);

            int magic = Reader.ReadInt32(false);
            Reader.SetPosition(position);

            Reader.IsBigEndian = magic == MAGIC_TEX;

            return magic;
        }

        public TexHeader GetTexHeader()
        {
            OpenReader();

            Reader.SetPosition(0);

            TexHeader header = new();

            header.Magic = Reader.ReadInt32(false);

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
                PixelFormatFourCC = Header.Format == 0x14 || Header.Format == 0x1E ? 0x31545844 : 0x35545844,

                Caps = 0x00001000
            };

        public FileInfo ExportDDS()
        {
            if (Header.Unknown1 == 0x06 || // Environment_CM.tex
               (Header.Format != 0x14 &&
                Header.Format != 0x18 &&
                Header.Format != 0x1E))
                return null;

            try
            {
                FileInfo output = new(Path.ChangeExtension(File.FullName, "dds"));
                using FileStream stream = output.OpenWrite();

                byte[] head = GetBytes(GetDDSHeader());
                byte[] data = GetPixelData();

                stream.SetLength(head.Length + data.Length);

                stream.Write(head, 0, head.Length);
                stream.Write(data, 0, data.Length);

                return output;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return null;
            }
        }

        public byte[] GetPixelData()
        {
            Reader.SetPosition(16 + Header.MipMapCount * 4);
            return Reader.ReadBytes((int)(File.Length - Reader.GetPosition()));
        }

        public void OpenReader()
        {
            if (Stream == null)
            {
                Stream = File.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                Reader = new(Stream);
            }
        }

        public void CloseReader()
        {
            if (Stream != null)
            {
                ((IDisposable)Stream).Dispose();
                Stream = null;
            }

            if (Reader != null)
            {
                ((IDisposable)Reader).Dispose();
                Reader = null;
            }
        }

        public void Dispose() =>
            CloseReader();

        private static byte[] GetBytes(DDSHeader data)
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
        public byte Unknown1;
        public byte AlphaFlags;

        public byte MipMapCount;
        public short Width;
        public short Height;

        public byte ImageCount;
        public byte Format;
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
