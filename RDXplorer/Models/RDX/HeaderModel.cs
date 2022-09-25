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
        public HeaderEntryModel Item { get; set; } = new("Item");
        public HeaderEntryModel Effect { get; set; } = new("Effect");
        public HeaderEntryModel Boundary { get; set; } = new("Boundary");
        public HeaderEntryModel Door { get; set; } = new("Door");
        public HeaderEntryModel Trigger { get; set; } = new("Trigger");
        public HeaderEntryModel Player { get; set; } = new("Player");
        public HeaderEntryModel Event { get; set; } = new("Event");
        public HeaderEntryModel Unknown1 { get; set; } = new("Unknown1");
        public HeaderEntryModel Unknown2 { get; set; } = new("Unknown2");
        public HeaderEntryModel Action { get; set; } = new("Action");
        public HeaderEntryModel Text { get; set; } = new("Text");
        public HeaderEntryModel Sysmes { get; set; } = new("Sysmes");

        public HeaderModel()
        {
            Version.IsPointer = false;
            Author.IsPointer = false;
            Unknown1.IsPointer = false;
            Unknown2.IsPointer = false;

            Author.IsText = true;

            Version.HasCount = false;
            Author.HasCount = false;
            Scripts.HasCount = false;
            Model.HasCount = false;
            Motion.HasCount = false;
            Flags.HasCount = false;
            Texture.HasCount = false;
            Text.HasCount = false;
            Sysmes.HasCount = false;
        }
    }
}