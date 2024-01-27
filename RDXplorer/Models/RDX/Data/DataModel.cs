using System;

namespace RDXplorer.Models.RDX
{
    public class DataModel<T> : IDataModel<T>
        where T : IFieldsModel, new()
    {
        public IntPtr Position { get; set; }
        public uint Size { get; set; }
        public T Fields { get; private set; } = new();
    }
}