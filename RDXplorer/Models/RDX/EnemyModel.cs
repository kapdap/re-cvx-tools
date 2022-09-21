namespace RDXplorer.Models.RDX
{
    public class EnemyModel
    {
        public long Offset;

        public DataEntryModel<int> Unknown0 { get; set; } = new DataEntryModel<int>();
        public DataEntryModel<short> Type { get; set; } = new DataEntryModel<short>();
        public DataEntryModel<short> Unknown1 { get; set; } = new DataEntryModel<short>();
        public DataEntryModel<byte> Unknown2 { get; set; } = new DataEntryModel<byte>();
        public DataEntryModel<byte> Unknown3 { get; set; } = new DataEntryModel<byte>();
        public DataEntryModel<byte> Slot { get; set; } = new DataEntryModel<byte>();
        public DataEntryModel<byte> Unknown4 { get; set; } = new DataEntryModel<byte>();
        public DataEntryModel<float> X { get; set; } = new DataEntryModel<float>();
        public DataEntryModel<float> Y { get; set; } = new DataEntryModel<float>();
        public DataEntryModel<float> Z { get; set; } = new DataEntryModel<float>();
        public DataEntryModel<int> Unknown5 { get; set; } = new DataEntryModel<int>();
        public DataEntryModel<short> Rotation { get; set; } = new DataEntryModel<short>();
        public DataEntryModel<short> Unknown6 { get; set; } = new DataEntryModel<short>();
        public DataEntryModel<int> Unknown7 { get; set; } = new DataEntryModel<int>();
    }
}