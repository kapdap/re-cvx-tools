namespace RDXplorer.Models.RDX
{
    public class HeaderEntryModel : DataEntryModel<uint>
    {
        public DataEntryModel<uint> Count { get; set; } = new DataEntryModel<uint>();
    }
}