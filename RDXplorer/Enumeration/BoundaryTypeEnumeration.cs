namespace RDXplorer.Enumerations
{
    public enum BoundaryTypeEnumeration : byte
    {
        Box = 0x00,
        BoxConditional = 0x01,
        Cylinder = 0x02,
        CylinderConditional = 0x03,
        Slope = 0x04,
        SlopeConditional = 0x05,
        Region = 0x06,
        Step = 0x07
    }
}
