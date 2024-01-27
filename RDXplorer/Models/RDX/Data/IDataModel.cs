using System;

namespace RDXplorer.Models.RDX
{
    public interface IBaseModel
    {
        IntPtr Position { get; set; }
        uint Size { get; set; }
    }

    public interface IDataModel<T> : IBaseModel
        where T : IFieldsModel, new()
    {
        T Fields { get; }
    }

    public interface IFieldsModel
    {
    }
}
