using System.IO;

namespace RDXplorer.Formats.TIM2
{
    public class Tim2Mipmap
    {
        public int Miptbp1 { get; set; }
        public int Miptbp2 { get; set; }
        public int Miptbp3 { get; set; }
        public int Miptbp4 { get; set; }
        public int[] Sizes { get; set; } = new int[8];

        public Tim2Mipmap(BinaryReader reader)
        {
            Miptbp1 = reader.ReadInt32();
            Miptbp2 = reader.ReadInt32();
            Miptbp3 = reader.ReadInt32();
            Miptbp4 = reader.ReadInt32();
            for (int i = 0; i < 8; i++)
                Sizes[i] = reader.ReadInt32();
        }
    }
}
