namespace RDXplorer.Models.RDX
{
    public class MotionBlockModel : DataModel<MotionBlockModelFields>
    {
        public MotionTableModel Table { get; set; }
    }

    public class MotionBlockModelFields : IFieldsModel
    {
        public DataEntryModel<uint> Type { get; set; } = new();
        public DataEntryModel<uint> Size { get; set; } = new();
        public DataEntryModel<uint> Data { get; set; } = new();
    }
}