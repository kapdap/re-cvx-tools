using System.IO;
using System.Text;

namespace RDXplorer.Formats.TIM2
{
    public class Tim2Header
    {
        public string MagicCode { get; set; } 
        public byte FileVersion { get; set; }
        public byte Format { get; set; }
        public short PictureCount { get; set; }
        public int Reserved1 { get; set; }
        public int Reserved2 { get; set; }

        public Tim2Header(BinaryReader reader)
        {
            MagicCode = Encoding.ASCII.GetString(reader.ReadBytes(4));
            if (MagicCode != "TIM2")
                throw new InvalidDataException("Not a TIM2 file. Invalid magic code.");
            FileVersion = reader.ReadByte();
            Format = reader.ReadByte();
            PictureCount = reader.ReadInt16();
            Reserved1 = reader.ReadInt32();
            Reserved2 = reader.ReadInt32(); 
            reader.BaseStream.Seek(112, SeekOrigin.Current);
        }
    }
}
