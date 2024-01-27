namespace RDXplorer.Models.RDX
{
    public class LightingModel : DataModel<LightingModelFields> { }

    public class LightingModelFields : IFieldsModel
    {
        public DataEntryModel<short> Unknown1 { get; set; } = new();
        public DataEntryModel<short> Unknown2 { get; set; } = new();
        public DataEntryModel<int> Unknown3 { get; set; } = new();
        public DataEntryModel<int> Unknown4 { get; set; } = new();
        public DataEntryModel<int> Unknown5 { get; set; } = new();
        public DataEntryModel<int> Unknown6 { get; set; } = new();
        public DataEntryModel<int> Unknown7 { get; set; } = new();
        public DataEntryModel<int> Unknown8 { get; set; } = new();
        public DataEntryModel<float> Unknown9 { get; set; } = new();
        public DataEntryModel<float> Unknown10 { get; set; } = new();
        public DataEntryModel<float> Unknown11 { get; set; } = new();
        public DataEntryModel<float> Unknown12 { get; set; } = new();
        public DataEntryModel<float> Unknown13 { get; set; } = new();
        public DataEntryModel<int> Data { get; set; } = new();
    }
}