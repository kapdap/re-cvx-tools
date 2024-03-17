using System;
using System.IO;

namespace ARCVX.Formats
{
    public class Mes : Base<MesHeader>
    {
        public override int MAGIC { get; } = 0;
        public override int MAGIC_LE { get; } = 0;

        public Mes(FileInfo file) : base(file) { }
        public Mes(FileInfo file, Stream stream) : base(file, stream) { }

        public override int GetMagic() =>
            0;

        public override MesHeader GetHeader() =>
            new();

        public FileInfo Export() =>
            throw new NotImplementedException("Export has not been implemented");
    }

    public struct MesHeader { }
}