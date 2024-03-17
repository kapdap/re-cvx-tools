using System;
using System.IO;

namespace ARCVX.Formats
{
    public class Evt : Base<EvtHeader>
    {
        public override int MAGIC { get; } = 0;
        public override int MAGIC_LE { get; } = 0;

        public Evt(FileInfo file) : base(file) { }
        public Evt(FileInfo file, Stream stream) : base(file, stream) { }

        public override int GetMagic() =>
            0;

        public override EvtHeader GetHeader() =>
            new();

        public FileInfo Export() =>
            throw new NotImplementedException("Export has not been implemented");
    }

    public struct EvtHeader { }
}