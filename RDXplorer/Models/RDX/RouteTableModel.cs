namespace RDXplorer.Models.RDX
{
    public class RouteTableModel : DataModel<RouteTableModelFields> { }

    public class RouteTableModelFields : IFieldsModel
    {
        public DataEntryModel<byte> Value { get; set; } = new();
    }
}
