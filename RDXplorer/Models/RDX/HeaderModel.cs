namespace RDXplorer.Models.RDX
{
    public class HeaderModel
    {
        public HeaderEntryModel Version { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Author { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Scripts { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Model { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Motion { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Flags { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Texture { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Camera { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Lighting { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Enemy { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Object { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Unknown1 { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Unknown2 { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel SCA { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel AOT { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Unknown3 { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Player { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Unknown4 { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Unknown5 { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Unknown6 { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Event { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Text { get; set; } = new HeaderEntryModel();
        public HeaderEntryModel Sysmes { get; set; } = new HeaderEntryModel();
    }
}