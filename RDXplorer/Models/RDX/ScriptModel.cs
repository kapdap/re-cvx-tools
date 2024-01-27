namespace RDXplorer.Models.RDX
{
    public class ScriptModel : DataModel<ScriptModelFields>
    {
        public DataEntryModel<uint> Pointer { get; set; } = new() { IsPointer = true };
    }

    public class ScriptModelFields : IFieldsModel
    {
        public DataEntryModel<int> Offset { get; set; } = new();
        public DataEntryModel<byte> Data { get; set; } = new();
    }
}