namespace RDXplorer.Models.RDX
{
    public class EffectModel : DataModel<EffectModelFields> { }

    public class EffectModelFields : IFieldsModel
    {
        public DataEntryModel<int> Unknown1 { get; set; } = new();
        public DataEntryModel<short> Unknown2 { get; set; } = new();
        public DataEntryModel<short> Unknown3 { get; set; } = new();
        public DataEntryModel<int> Unknown4 { get; set; } = new();
        public DataEntryModel<float> X { get; set; } = new();
        public DataEntryModel<float> Y { get; set; } = new();
        public DataEntryModel<float> Z { get; set; } = new();
        public DataEntryModel<float> Width { get; set; } = new();
        public DataEntryModel<float> Height { get; set; } = new();
        public DataEntryModel<float> Length { get; set; } = new();
        public DataEntryModel<int> Unknown11 { get; set; } = new();
        public DataEntryModel<int> Unknown12 { get; set; } = new();
        public DataEntryModel<int> Unknown13 { get; set; } = new();
        public DataEntryModel<int> Unknown14 { get; set; } = new();
        public DataEntryModel<float> Unknown15 { get; set; } = new();
        public DataEntryModel<float> Unknown16 { get; set; } = new();
        public DataEntryModel<float> Unknown17 { get; set; } = new();
        public DataEntryModel<int> Unknown18 { get; set; } = new();
    }
}