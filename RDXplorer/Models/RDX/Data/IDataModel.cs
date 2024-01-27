using System;

namespace RDXplorer.Models.RDX
{
    public interface IDataModel<T>
        where T : IFieldsModel, new()
    {
        IntPtr Position { get; set; }
        uint Size { get; set; }
        T Fields { get; }
    }

    public interface IFieldsModel
    {
    }
}
