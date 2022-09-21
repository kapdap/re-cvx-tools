namespace RDXplorer.Models.RDX
{
    public class DataEntryModel<T>
    {
        public long Offset { get; set; }
        public long Size => Data != null ? Data.LongLength : 0;
        public byte[] Data { get; set; }
        public T Value { get; set; }
        public string Text { get; set; }
        public bool IsPointer { get; set; } = false;

        public DataEntryModel() { }

        public DataEntryModel(bool isPointer = false)
        {
            IsPointer = isPointer;
        }
    }
}