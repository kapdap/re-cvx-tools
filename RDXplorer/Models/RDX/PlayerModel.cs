namespace RDXplorer.Models.RDX
{
    // On-disk layout: POS (types.h) — 0x10 bytes.
    public class PlayerModel : DataModel<PlayerModelFields> { }

    public class PlayerModelFields : IFieldsModel
    {
        public DataEntryModel<float> Px { get; set; } = new();
        public DataEntryModel<float> Py { get; set; } = new();
        public DataEntryModel<float> Pz { get; set; } = new();
        public DataEntryModel<int> Ay { get; set; } = new();
    }
}
