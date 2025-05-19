using System.IO;

namespace RDXplorer.Formats.TIM2
{
    public class Tim2Picture
    {
        public int TotalSize { get; set; }
        public int ClutSize { get; set; }
        public int ImageSize { get; set; }
        public short HeaderSize { get; set; }
        public short ClutColorCount { get; set; }
        public byte PictureFormat { get; set; }
        public byte MipmapCount { get; set; }
        public Tim2ColorType ClutColorType { get; set; }
        public Tim2ColorType ImageColorType { get; set; }
        public short Width { get; set; }
        public short Height { get; set; }
        public Tim2GsTex GsTex0 { get; set; }
        public Tim2GsTex GsTex1 { get; set; }
        public int GsFlags { get; set; } 
        public int GsClut { get; set; }  

        public Tim2Mipmap MipmapData { get; set; }
        public byte[] ImageData { get; set; }
        public byte[] ClutData { get; set; }

        public Tim2Picture(BinaryReader reader)
        {
            long start = reader.BaseStream.Position;

            TotalSize = reader.ReadInt32();
            ClutSize = reader.ReadInt32();
            ImageSize = reader.ReadInt32();
            HeaderSize = reader.ReadInt16();
            ClutColorCount = reader.ReadInt16();
            PictureFormat = reader.ReadByte();
            MipmapCount = reader.ReadByte();
            ClutColorType = (Tim2ColorType)reader.ReadByte();
            ImageColorType = (Tim2ColorType)reader.ReadByte();
            Width = reader.ReadInt16();
            Height = reader.ReadInt16();
            GsTex0 = new Tim2GsTex(reader.ReadInt64());
            GsTex1 = new Tim2GsTex(reader.ReadInt64());
            GsFlags = reader.ReadInt32();
            GsClut = reader.ReadInt32();

            if (MipmapCount > 1 && reader.BaseStream.Position - start < HeaderSize)
                MipmapData = new Tim2Mipmap(reader);

            long hdrRead = reader.BaseStream.Position - start;
            if (hdrRead < HeaderSize)
                reader.BaseStream.Seek(HeaderSize - hdrRead, SeekOrigin.Current);

            ImageData = reader.ReadBytes(ImageSize);
            ClutData = ClutSize > 0 ? reader.ReadBytes(ClutSize) : null;

            long end = start + TotalSize;
            if (reader.BaseStream.Position != end)
                reader.BaseStream.Seek(end - reader.BaseStream.Position, SeekOrigin.Current);
        }
    }
}
