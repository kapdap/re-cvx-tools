namespace RDXplorer.Models.RDX
{
    public class HeaderEntryModel : DataEntryModel<uint>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public override bool IsPointer { get; set; } = true;

        public virtual bool HasCount { get; set; } = true;
        public DataEntryModel<uint> Count { get; set; } = new DataEntryModel<uint>();

        public HeaderEntryModel(string name) =>
            Name = name;

        public HeaderEntryModel(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}