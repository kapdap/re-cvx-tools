using RECVXFlagTool.Models.Base;
using System;

namespace RECVXFlagTool.Models
{
    public class FlagModel : DataModel<bool>
    {
        public IntPtr FlagPointer { get; set; }

        public int Offset { get; set; }
        public int Bit { get; set; }

        public int Index { get; set; }
        public int Order { get; set; }

        public string Flag { get; set; }
        public string Name { get; set; }
    }
}
