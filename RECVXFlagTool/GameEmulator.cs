using RECVXFlagTool.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace RECVXFlagTool
{
    public class GameEmulator : IDisposable
    {
        // TODO: Add Retroarch support
        public const string RPCS3 = "rpcs3";
        public const string Dolphin = "dolphin";
        public const string Flycast = "flycast";
        public const string Redream = "redream";
        public const string NullDC = "nulldc";
        public const string Demul = "demul";

        public const string PCSX2 = "pcsx2";
        public const string PCSX2QT = "pcsx2-qt";
        public const string PCSX264QT = "pcsx2-qtx64";
        public const string PCSX264QTAV = "pcsx2-qtx64-avx2";
        public const string PCSX264WX = "pcsx2x64";
        public const string PCSX264WXAV = "pcsx2x64-avx2";

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
            else if (Process.ProcessName.ToLower().StartsWith(PCSX2))
            {
                UpdateVirtualMemoryPointer();
                ProductLength = 11;
                IsBigEndian = false;
            }
            else if (Process.ProcessName.ToLower() == Demul)
            {
                VirtualMemoryPointer = new IntPtr(0x2B000000);
                ProductPointer = IntPtr.Add(VirtualMemoryPointer, 0x01008040);
                ProductLength = 7;
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
            else // RPCS3
            {
                UpdateVirtualMemoryPointer();
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
                IntPtr pointer = IntPtr.Zero;

                _dolphin ??= new Dolphin.Memory.Access.Dolphin(Process);
                _dolphin.TryGetBaseAddress(out pointer);

                VirtualMemoryPointer = pointer;
                ProductPointer = IntPtr.Add(VirtualMemoryPointer, 0x0);
            }
            else if (Process.ProcessName.ToLower().StartsWith(PCSX2))
            {
                if (Process.ProcessName.ToLower() == PCSX2) // PCSX2 1.6 and earlier
                {
                    VirtualMemoryPointer = new IntPtr(0x20000000);
                    ProductPointer = IntPtr.Add(VirtualMemoryPointer, 0x00015B90);
                }
                else // PCSX2 1.7+
                {
                    try
                    {
                        // https://forums.pcsx2.net/Thread-PCSX2-1-7-Cheat-Engine-Script-Compatibility
                        IntPtr process = NativeWrappers.LoadLibrary(Process.MainModule.FileName);
                        IntPtr address = NativeWrappers.GetProcAddress(process, "EEmem");

                        VirtualMemoryPointer = (IntPtr)Process.ReadValue<long>(address);

                        ProductPointer = Process.ProcessName.ToLower() == PCSX264WX ||
                            Process.ProcessName.ToLower() == PCSX264WXAV
                            ? IntPtr.Add(VirtualMemoryPointer, 0x000155D0)
                            : IntPtr.Add(VirtualMemoryPointer, 0x00012610);

                        NativeWrappers.FreeLibrary(process);
                    }
                    catch { }
                }
            }
            else // RPCS3
            {
                // TODO: Replace this with the code below.
                // TODO: Recent versions of RPCS3 use dynamic memory pointer.
                VirtualMemoryPointer = new IntPtr(0x300000000);
                ProductPointer = IntPtr.Add(VirtualMemoryPointer, 0x20010251);

                // TODO: Import Reloaded.Memory library
                // TODO: Update Dolphin.Memory library to prevent conflict with latest Reloaded.Memory
                /*Scanner scanner = new Scanner(Process, Process.MainModule);
                PatternScanResult result = scanner.FindPattern("50 53 33 5F 47 41 4D 45 00 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00 0F 00 00 00 00 00 00 00 30 30");

                IntPtr pointer = IntPtr.Add(Process.MainModule.BaseAddress, result.Offset);
                ProductPointer = result.Offset != 0 ? IntPtr.Add(pointer, -0xE0) : IntPtr.Zero;*/
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