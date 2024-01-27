namespace RDXplorer.Models.RDX
{
    public class HeaderModel
    {
        public HeaderEntryModel Version { get; set; } = new("Version") { HasCount = false, IsPointer = false};
        public HeaderEntryModel Author { get; set; } = new("Author") { HasCount = false, IsPointer = false, IsText = true };
        public HeaderEntryModel Tables { get; set; } = new("Tables") { HasCount = false };
        public HeaderEntryModel Model { get; set; } = new("Model") { HasCount = false };
        public HeaderEntryModel Motion { get; set; } = new("Motion") { HasCount = false };
        public HeaderEntryModel Script { get; set; } = new("Script") { HasCount = false };
        public HeaderEntryModel Texture { get; set; } = new("Texture");
        public HeaderEntryModel Camera { get; set; } = new("Camera");
        public HeaderEntryModel Lighting { get; set; } = new("Lighting");
        public HeaderEntryModel Actor { get; set; } = new("Actor");
        public HeaderEntryModel Object { get; set; } = new("Object");
        public HeaderEntryModel Item { get; set; } = new("Item");
        public HeaderEntryModel Effect { get; set; } = new("Effect");
        public HeaderEntryModel Boundary { get; set; } = new("Boundary");
        public HeaderEntryModel AOT { get; set; } = new("AOT");
        public HeaderEntryModel Trigger { get; set; } = new("Trigger");
        public HeaderEntryModel Player { get; set; } = new("Player");
        public HeaderEntryModel Event { get; set; } = new("Event");
        public HeaderEntryModel Unknown1 { get; set; } = new("Unknown 1");
        public HeaderEntryModel Unknown2 { get; set; } = new("Unknown 2") { IsPointer = false };
        public HeaderEntryModel Action { get; set; } = new("Action");
        public HeaderEntryModel Text { get; set; } = new("Text") { HasCount = false };
        public HeaderEntryModel Sysmes { get; set; } = new("Sysmes") { HasCount = false };
    }
}