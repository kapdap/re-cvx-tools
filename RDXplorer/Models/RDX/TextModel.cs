namespace RDXplorer.Models.RDX
{
    public class TextModel : DataModel<TextModelFields>
    {
        public DataEntryModel<uint> Pointer { get; set; } = new() { IsPointer = true };
        public string Message { get; set; }
    }

    public class TextModelFields : IFieldsModel
    {
        public DataEntryModel<int> Offset { get; set; } = new();
        public DataEntryModel<byte> Data { get; set; } = new();
    }
}