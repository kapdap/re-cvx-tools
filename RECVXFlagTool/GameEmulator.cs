using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace RECVXFlagTool
{
    public class GameEmulator : IDisposable
    {
        public const string RPCS3 = "rpcs3";
        public const string PCSX2 = "pcsx2";
        public const string Dolphin = "dolphin";
        public const string Flycast = "flycast";
        public const string Redream = "redream";
        public const string NullDC = "nulldc";
        public const string Demul = "demul";

        private Dolphin.Memory.Access.Dolphin _dolphin;

        private static List<string> _emulatorList;
        public static List<string> EmulatorList
        {
            get
            {
                if (_emulatorList == null)
                {
                    _emulatorList = new List<string>();

                    foreach (FieldInfo field in typeof(GameEmulator).GetFields())
                        if (field.IsLiteral && !field.IsInitOnly)
                            _emulatorList.Add((string)field.GetValue(null));
                }

                return _emulatorList;
            }
        }

        public Process Process { get; private set; }

        public IntPtr VirtualMemoryPointer { get; private set; }
        public IntPtr ProductPointer { get; private set; }
        public int ProductLength { get; private set; }
        public bool IsBigEndian { get; private set; }

        public GameEmulator(Process process)
        {
            if ((Process = process) == null)
                return;

            if (Process.HasExited)
                return;

            if (Process.ProcessName.ToLower() == Dolphin)
            {
                UpdateVirtualMemoryPointer();
                ProductLength = 6;
                IsBigEndian = true;
            }
            else if (Process.ProcessName.ToLower() == PCSX2)
            {
                VirtualMemoryPointer = new IntPtr(0x20000000);
                ProductPointer = IntPtr.Add(VirtualMemoryPointer, 0x00015B90);
                ProductLength = 11;
                IsBigEndian = false;
            }
            // TODO: Find a consistant way to get Flycast/Redream virtual memory pointers
            /*else if (Process.ProcessName.ToLower() == Flycast)
            {
                VirtualMemoryPointer = new IntPtr(0x0);
                ProductPointer = IntPtr.Add(VirtualMemoryPointer, 0x0);
                ProductLength = 7;
                IsBigEndian = false;
            }
            else if (Process.ProcessName.ToLower() == Redream)
            {
                VirtualMemoryPointer = new IntPtr(0x0);
                ProductPointer = IntPtr.Add(VirtualMemoryPointer, 0x0);
                ProductLength = 7;
                IsBigEndian = false;
            }
            else if (Process.ProcessName.ToLower() == NullDC)
            {
                VirtualMemoryPointer = new IntPtr(0x0);
                ProductPointer = IntPtr.Add(VirtualMemoryPointer, 0x0);
                ProductLength = 7;
                IsBigEndian = false;
            }*/
            else if (Process.ProcessName.ToLower() == Demul)
            {
                VirtualMemoryPointer = new IntPtr(0x2B000000);
                ProductPointer = IntPtr.Add(VirtualMemoryPointer, 0x01008040);
                ProductLength = 7;
                IsBigEndian = false;
            }
            else // RPCS3
            {
                VirtualMemoryPointer = new IntPtr(0x300000000);
                ProductPointer = IntPtr.Add(VirtualMemoryPointer, 0x20010251);
                ProductLength = 9;
                IsBigEndian = true;
            }
        }

        public void UpdateVirtualMemoryPointer()
        {
            if (Process == null)
                return;

            if (Process.HasExited)
                return;

            if (Process.ProcessName.ToLower() == Dolphin)
            {
                _dolphin ??= new Dolphin.Memory.Access.Dolphin(Process);
                _dolphin.TryGetBaseAddress(out IntPtr pointer);

                VirtualMemoryPointer = pointer;
                ProductPointer = IntPtr.Add(VirtualMemoryPointer, 0x0);
            }
        }

        public static GameEmulator DetectEmulator()
        {
            foreach (string name in EmulatorList)
            {
                Process[] processes = Process.GetProcessesByName(name);

                if (processes.Length <= 0)
                    continue;

                return new GameEmulator(processes[0]);
            }

            return null;
        }

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Process?.Dispose();
                    Process = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}