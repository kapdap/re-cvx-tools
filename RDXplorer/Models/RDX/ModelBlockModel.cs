namespace RDXplorer.Models.RDX
{
    public class ModelBlockModel : DataModel<ModelBlockFieldsModel>
    {
        public ModelTableModel Table { get; set; }
        
        public bool HasSize { get; set; } = false;
    }

    public class ModelBlockFieldsModel : IFieldsModel
    {
        public DataEntryModel<uint> Type { get; set; } = new();
        public DataEntryModel<uint> Size { get; set; } = new();
    }
}