namespace RDXplorer.Models.RDX
{
    public class PlayerModel : DataModel<PlayerModelFields> { }

    public class PlayerModelFields : IFieldsModel
    {
        public DataEntryModel<float> X { get; set; } = new();
        public DataEntryModel<float> Y { get; set; } = new();
        public DataEntryModel<float> Z { get; set; } = new();
        public DataEntryModel<int> Rotation { get; set; } = new();
    }
}