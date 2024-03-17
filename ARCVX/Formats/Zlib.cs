namespace ARCVX.Formats
{
    // https://www.rfc-editor.org/rfc/rfc1950
    // https://stackoverflow.com/a/54915442
    public struct ZlibHeader
    {
        public int CINFO;
        public int CM;
        public int FLEVEL;
        public int FDICT;
        public int FCHECK;

        public bool IsValid() =>
            CINFO >= 0 && CINFO <= 7 && CM == 8 && FLEVEL >= 0 && FLEVEL <= 3 && FDICT >= 0 && FDICT <= 1;

        public ZlibHeader(short data)
        {
            CINFO = (data >> 12) & 0xF;
            CM = (data >> 8) & 0xF;

            FLEVEL = (data >> 6) & 0x3;
            FDICT = (data >> 5) & 0x1;
            FCHECK = data & 0x1F;
        }

        public ZlibHeader(int cmf, int flg) : this((byte)cmf, (byte)flg) { }

        public ZlibHeader(byte cmf, byte flg)
        {
            CINFO = (cmf >> 4) & 0xF;
            CM = cmf & 0xF;

            FLEVEL = (flg >> 6) & 0x3;
            FDICT = (flg >> 5) & 0x1;
            FCHECK = flg & 0x1F;
        }
    }
}