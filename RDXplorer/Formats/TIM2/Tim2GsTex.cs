namespace RDXplorer.Formats.TIM2
{
    public class Tim2GsTex(long rawGsTex)
    {
        public ulong TextureBasePointer { get; set; } = (ulong)(rawGsTex & 0x3FFF) * 0x100;
        public uint TextureBufferWidth { get; set; } = (uint)(rawGsTex >> 14 & 0x3F);
        public Tim2PixelStorageMode PixelStorageFormat { get; set; } = (Tim2PixelStorageMode)(rawGsTex >> 20 & 0x3F);
        public uint TextureWidthLog2 { get; set; } = (uint)(rawGsTex >> 26 & 0xF);
        public uint TextureHeightLog2 { get; set; } = (uint)(rawGsTex >> 30 & 0xF);
        public bool HasAlpha { get; set; } = (rawGsTex >> 34 & 0x1) == 1;
        public Tim2TextureFunction TextureFunctionValue { get; set; } = (Tim2TextureFunction)(rawGsTex >> 35 & 0x3);
        public ulong ClutBasePointer { get; set; } = (ulong)(rawGsTex >> 37 & 0x3FFF) * 0x100;
        public Tim2ClutPixelStorageMode ClutPixelStorageFormat { get; set; } = (Tim2ClutPixelStorageMode)(rawGsTex >> 51 & 0xF);
        public Tim2ColorStorageMode ClutStorageModeValue { get; set; } = (Tim2ColorStorageMode)(rawGsTex >> 55 & 0x1);
        public uint ClutEntryOffset { get; set; } = (uint)(rawGsTex >> 56 & 0x1F);
        public uint LoadControl { get; set; } = (uint)(rawGsTex >> 61 & 0x7);
    }
}
