using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RDXplorer.Formats.TIM2
{
    public class Tim2Document
    {
        public Tim2Header Header { get; set; }
        public List<Tim2Picture> Pictures { get; set; } = [];

        public Tim2Document(string path)
        {
            using FileStream fs = new(path, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new(fs);

            Header = new(reader);

            for (int i = 0; i < Header.PictureCount; i++)
                Pictures.Add(new(reader));
        }

        public Tim2Document(Stream stream)
        {
            using BinaryReader reader = new(stream, Encoding.ASCII);

            Header = new(reader);

            for (int i = 0; i < Header.PictureCount; i++)
                Pictures.Add(new(reader));
        }

        public Tim2Document(byte[] data)
            : this(new MemoryStream(data)) 
        {
        }
    }
}
