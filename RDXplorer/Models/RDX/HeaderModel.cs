namespace RDXplorer.Models.RDX
{
    public class HeaderModel
    {
        public HeaderEntryModel Version { get; set; } = new("Version");
        public HeaderEntryModel Author { get; set; } = new("Author");
        public HeaderEntryModel Scripts { get; set; } = new("Scripts");
        public HeaderEntryModel Model { get; set; } = new("Model");
        public HeaderEntryModel Motion { get; set; } = new("Motion");
        public HeaderEntryModel Flags { get; set; } = new("Flags");
        public HeaderEntryModel Texture { get; set; } = new("Texture");
        public HeaderEntryModel Camera { get; set; } = new("Camera");
        public HeaderEntryModel Lighting { get; set; } = new("Lighting");
        public HeaderEntryModel Enemy { get; set; } = new("Enemy");
        public HeaderEntryModel Object { get; set; } = new("Object");
        public HeaderEntryModel Unknown1 { get; set; } = new("Item");
        public HeaderEntryModel Unknown2 { get; set; } = new("Effect");
        public HeaderEntryModel SCA { get; set; } = new("Boundary");
        public HeaderEntryModel AOT { get; set; } = new("Door");
        public HeaderEntryModel Unknown3 { get; set; } = new("Trigger");
        public HeaderEntryModel Player { get; set; } = new("Player");
        public HeaderEntryModel Unknown4 { get; set; } = new("Event");
        public HeaderEntryModel Unknown5 { get; set; } = new("Unknown5");
        public HeaderEntryModel Unknown6 { get; set; } = new("Unknown6");
        public HeaderEntryModel Event { get; set; } = new("Action");
        public HeaderEntryModel Text { get; set; } = new("Text");
        public HeaderEntryModel Sysmes { get; set; } = new("Sysmes");

        public HeaderModel()
        {
            Version.IsPointer = false;
            Author.IsPointer = false;
            Unknown5.IsPointer = false;

            Author.IsText = true;

            Version.HasCount = false;
            Author.HasCount = false;
            Scripts.HasCount = false;
            Model.HasCount = false;
            Motion.HasCount = false;
            Flags.HasCount = false;
            Texture.HasCount = false;
        }
    }
}