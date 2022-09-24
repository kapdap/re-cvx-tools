namespace RDXplorer.Models.RDX
{
    public class HeaderModel
    {
        public HeaderEntryModel Version { get; set; } = new HeaderEntryModel("Version");
        public HeaderEntryModel Author { get; set; } = new HeaderEntryModel("Author");
        public HeaderEntryModel Scripts { get; set; } = new HeaderEntryModel("Scripts");
        public HeaderEntryModel Model { get; set; } = new HeaderEntryModel("Model");
        public HeaderEntryModel Motion { get; set; } = new HeaderEntryModel("Motion");
        public HeaderEntryModel Flags { get; set; } = new HeaderEntryModel("Flags");
        public HeaderEntryModel Texture { get; set; } = new HeaderEntryModel("Texture");
        public HeaderEntryModel Camera { get; set; } = new HeaderEntryModel("Camera");
        public HeaderEntryModel Lighting { get; set; } = new HeaderEntryModel("Lighting");
        public HeaderEntryModel Enemy { get; set; } = new HeaderEntryModel("Enemy");
        public HeaderEntryModel Object { get; set; } = new HeaderEntryModel("Object");
        public HeaderEntryModel Unknown1 { get; set; } = new HeaderEntryModel("Item");
        public HeaderEntryModel Unknown2 { get; set; } = new HeaderEntryModel("Effect");
        public HeaderEntryModel SCA { get; set; } = new HeaderEntryModel("Boundary");
        public HeaderEntryModel AOT { get; set; } = new HeaderEntryModel("Door");
        public HeaderEntryModel Unknown3 { get; set; } = new HeaderEntryModel("Trigger");
        public HeaderEntryModel Player { get; set; } = new HeaderEntryModel("Player");
        public HeaderEntryModel Unknown4 { get; set; } = new HeaderEntryModel("Event");
        public HeaderEntryModel Unknown5 { get; set; } = new HeaderEntryModel("Unknown5");
        public HeaderEntryModel Unknown6 { get; set; } = new HeaderEntryModel("Unknown6");
        public HeaderEntryModel Event { get; set; } = new HeaderEntryModel("Action");
        public HeaderEntryModel Text { get; set; } = new HeaderEntryModel("Text");
        public HeaderEntryModel Sysmes { get; set; } = new HeaderEntryModel("Sysmes");

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