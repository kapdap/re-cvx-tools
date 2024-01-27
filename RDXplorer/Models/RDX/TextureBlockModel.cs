namespace RDXplorer.Models.RDX
{
    public class TextureBlockModel : DataModel<TextureBlockModelFields>
    {
        public TextureTableModel Table { get; set; }
    }

    public class TextureBlockModelFields : IFieldsModel
    {
        public DataEntryModel<uint> Type { get; set; } = new();
        public DataEntryModel<uint> Size { get; set; } = new();
    }
}