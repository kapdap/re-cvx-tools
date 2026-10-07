namespace RDXplorer.Models.RDX
{
    public class HeaderModel
    {
        public HeaderEntryModel Version { get; set; } = new("Version") { HasCount = false, IsPointer = false };
        public HeaderEntryModel Author { get; set; } = new("Author") { HasCount = false, IsPointer = false, IsText = true };
        public HeaderEntryModel Tables { get; set; } = new("Tables") { HasCount = false };
        public HeaderEntryModel Model { get; set; } = new("Model", "MDL/SKIN/MASK blobs") { HasCount = false };
        public HeaderEntryModel Motion { get; set; } = new("Motion", "MNB animation data") { HasCount = false };
        public HeaderEntryModel Script { get; set; } = new("Script", "Room SCD script code") { HasCount = false };
        public HeaderEntryModel Texture { get; set; } = new("Texture");
        public HeaderEntryModel Camera { get; set; } = new("Camera");
        public HeaderEntryModel Lighting { get; set; } = new("Lighting");
        public HeaderEntryModel Enemy { get; set; } = new("Enemy");
        public HeaderEntryModel Object { get; set; } = new("Object");
        public HeaderEntryModel Item { get; set; } = new("Item");
        public HeaderEntryModel Effect { get; set; } = new("Effect");
        public HeaderEntryModel Boundary { get; set; } = new("Boundary");
        public HeaderEntryModel AOT { get; set; } = new("AOT", "Interactive trigger zones");
        public HeaderEntryModel Trigger { get; set; } = new("Trigger", "Event trigger zones");
        public HeaderEntryModel Player { get; set; } = new("Player", "Player spawn positions");
        public HeaderEntryModel Route { get; set; } = new("Route", "Pathfinding route zones");
        public HeaderEntryModel RouteTable { get; set; } = new("Route Table") { HasCount = false };
        public HeaderEntryModel EventScript { get; set; } = new("Event Script", "Event scripts") { HasCount = false };
        public HeaderEntryModel EventCamera { get; set; } = new("Event Camera", "Event cameras");
        public HeaderEntryModel Text { get; set; } = new("Text", "Text messages");
        public HeaderEntryModel EventLight { get; set; } = new("Event Light", "Event lighting");
    }
}