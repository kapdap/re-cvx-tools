using System;

namespace RDXplorer.Models.RDX
{
    public class PlayerModel
    {
        public IntPtr Offset { get; set; }

        public DataEntryModel<float> X { get; set; } = new DataEntryModel<float>();
        public DataEntryModel<float> Y { get; set; } = new DataEntryModel<float>();
        public DataEntryModel<float> Z { get; set; } = new DataEntryModel<float>();
        public DataEntryModel<int> Rotation { get; set; } = new DataEntryModel<int>();
    }
}