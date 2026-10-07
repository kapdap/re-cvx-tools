namespace RDXplorer.Formats.PVR
{
    public enum PvrPixelFormat : byte
    {
        Argb1555 = 0x00,
        Rgb565 = 0x01,
        Argb4444 = 0x02
    }

    public enum PvrDataFormat : byte
    {
        Twiddled = 0x01,
        TwiddledMipmap = 0x02,
        Vq = 0x03,
        Pal4 = 0x05,
        Pal8 = 0x07,
        Rectangle = 0x09,
        RectTwiddled = 0x0D,
        SmallVq = 0x10
    }
}
