namespace RDXplorer.Models.RDX
{
    public interface IValueEntryModel<T> : IDataEntryModel
         where T : struct
    {
        public T Value { get; }
    }
}