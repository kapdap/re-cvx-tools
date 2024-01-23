using RECVXFlagTool.Models;
using System;

namespace RECVXFlagTool
{
    public class GamePointers
    {
        public GameModel Version { get; } = new GameModel();

        public IntPtr Room { get; set; }

        public IntPtr DocumentFlags { get; set; }
        public IntPtr KilledFlags { get; set; }
        public IntPtr EventFlags { get; set; }
        public IntPtr ItemFlags { get; set; }
        public IntPtr MapFlags { get; set; }
    }
}