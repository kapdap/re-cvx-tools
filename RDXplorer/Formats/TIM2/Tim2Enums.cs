namespace RDXplorer.Formats.TIM2
{
    public enum Tim2PixelStorageMode : byte
    {
        PSMCT32 = 0,
        PSMCT24 = 1,
        PSMCT16 = 2,
        PSMCT16S = 10,
        PSMT8 = 19,
        PSMT4 = 20,
        PSMT8H = 27,
        PSMT4HL = 38,
        PSMT4HH = 44,
        PSMZ32 = 48,
        PSMZ24 = 49,
        PSMZ16 = 50,
        PSMZ16S = 58
    }

    public enum Tim2ClutPixelStorageMode : byte
    {
        PSMCT32 = 0,
        PSMCT24 = 1,
        PSMCT16 = 2,
        PSMCT16S = 10
    }

    public enum Tim2ColorStorageMode : byte
    {
        CSM1 = 0, 
        CSM2 = 1  
    }

    public enum Tim2ColorType : byte
    {
        Undefined = 0,
        RGBA16_A1B5G5R5 = 1,
        RGB32_X8B8G8R8 = 2,
        RGBA32_A8B8G8R8 = 3,
        Indexed4Bit = 4,
        Indexed8Bit = 5
    }

    public enum Tim2TextureFunction : byte
    {
        Modulate = 0,
        Decal = 1,
        Hilight = 2,
        Hilight2 = 3
    }
}
