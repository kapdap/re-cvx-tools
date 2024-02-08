using RECVXFlagTool.Collections;
using RECVXFlagTool.Models;
using RECVXFlagTool.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RECVXFlagTool
{
    public class GameMemoryScanner : IDisposable
    {
        public GameMemory Memory { get; } = new GameMemory();
        public GamePointers Pointers { get; } = new GamePointers();
        public GameEmulator Emulator { get; private set; }

        private Process _process = null;

        public bool ProcessRunning => _process != null && !_process.HasExited && _process.IsRunning();
        public int ProcessExitCode => _process != null ? _process.ExitCode() : 0;

        public GameMemoryScanner(GameEmulator emulator) => 
            Initialize(emulator);

        public void Initialize(GameEmulator emulator)
        {
            if ((Emulator = emulator) == null)
                return;

            _process = Emulator.Process;

            if (ProcessRunning) Update();
        }

        public void Update()
        {
            UpdateVirtualMemoryPointer();
            UpdateGameVersion();
            UpdatePointerAddresses();
        }

        public void UpdateVirtualMemoryPointer() => 
            Emulator.UpdateVirtualMemoryPointer();

        public void UpdateGameVersion() => 
            Memory.Game.Update(_process.ReadString(Emulator.ProductPointer, Emulator.ProductLength));

        public void UpdatePointerAddresses()
        {
            if (Memory.Game.Equals(Pointers.Version))
                return;

            Pointers.Version.Update(Memory.Game.Code);

            switch (Pointers.Version.Code)
            {
                case GameModel.T1207M:
                case GameModel.T1210M:
                    Pointers.Room = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x01215030);
                    Pointers.DocumentFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x01214EE4);
                    Pointers.KilledFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0121490C);
                    Pointers.EventFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0121480C);
                    Pointers.ItemFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0121488C);
                    Pointers.MapFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x012149CC);
                    break;

                case GameModel.T1204N:
                    Pointers.Room = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x01216790);
                    Pointers.DocumentFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x01216644);
                    Pointers.KilledFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0121606C);
                    Pointers.EventFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x01215F6C);
                    Pointers.ItemFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x01215FEC);
                    Pointers.MapFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0121612C);
                    break;

                case GameModel.T36806D:
                    Pointers.Room = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x01216CB0);
                    Pointers.DocumentFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x01216B64);
                    Pointers.KilledFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0121658C);
                    Pointers.EventFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0121648C);
                    Pointers.ItemFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0121650C);
                    Pointers.MapFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0121664C);
                    break;

                case GameModel.T1240M:
                    Pointers.Room = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0121A210);
                    Pointers.DocumentFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0121A0C4);
                    Pointers.KilledFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x01219AEC);
                    Pointers.EventFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x012199EC);
                    Pointers.ItemFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x01219A6C);
                    Pointers.MapFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x01219BAC);
                    break;

                case GameModel.GCDJ08:
                    Pointers.Room = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00438BB0);
                    Pointers.DocumentFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00438B68);
                    Pointers.KilledFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0043848C);
                    Pointers.EventFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0043838C);
                    Pointers.ItemFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0043840C);
                    Pointers.MapFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0043854C);
                    break;

                case GameModel.GCDE08:
                    Pointers.Room = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x004345D0);
                    Pointers.DocumentFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00434588);
                    Pointers.KilledFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00433EAC);
                    Pointers.EventFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00433DAC);
                    Pointers.ItemFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00433E2C);
                    Pointers.MapFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00433F6C);
                    break;

                case GameModel.GCDP08:
                    Pointers.Room = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00438B70);
                    Pointers.DocumentFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00438B28);
                    Pointers.KilledFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0043844C);
                    Pointers.EventFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0043834C);
                    Pointers.ItemFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x004383CC);
                    Pointers.MapFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0043850C);
                    break;

                case GameModel.SLPM_65022:
                    Pointers.Room = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x004314B4);
                    Pointers.DocumentFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0043146C);
                    Pointers.KilledFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00430D90);
                    Pointers.EventFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00430C90);
                    Pointers.ItemFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00430D10);
                    Pointers.MapFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00430E50);
                    break;

                case GameModel.SLUS_20184:
                    Pointers.Room = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x004339B4);
                    Pointers.DocumentFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0043396C);
                    Pointers.KilledFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00433290);
                    Pointers.EventFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00433190);
                    Pointers.ItemFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00433210);
                    Pointers.MapFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00433350);
                    break;

                case GameModel.SLES_50306:
                    Pointers.Room = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0044A1E4);
                    Pointers.DocumentFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x0044A19C);
                    Pointers.KilledFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00449AC0);
                    Pointers.EventFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x004499C0);
                    Pointers.ItemFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00449A40);
                    Pointers.MapFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00449B80);
                    break;

                case GameModel.NPUB30467:
                    Pointers.Room = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BB3DCC);
                    Pointers.DocumentFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BB3D84);
                    Pointers.KilledFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BB36A8);
                    Pointers.EventFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BB35A8);
                    Pointers.ItemFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BB3628);
                    Pointers.MapFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BB3768);
                    break;

                case GameModel.NPEB00553:
                    Pointers.Room = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BC40CC);
                    Pointers.DocumentFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BC4084);
                    Pointers.KilledFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BC39A8);
                    Pointers.EventFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BC38A8);
                    Pointers.ItemFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BC3928);
                    Pointers.MapFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BC3A68);
                    break;

                default: // GameVersion.NPJB00135
                    Pointers.Room = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BB3E4C);
                    Pointers.DocumentFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BB3E04);
                    Pointers.KilledFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BB3728);
                    Pointers.EventFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BB3628);
                    Pointers.ItemFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BB36A8);
                    Pointers.MapFlags = IntPtr.Add(Emulator.VirtualMemoryPointer, 0x00BB37E8);
                    break;
            }
        }

        public GameMemory Refresh()
        {
            RefreshRoom();
            RefreshFlags();

            return Memory;
        }

        public void RefreshRoom() =>
            Memory.Room.Id = _process.ReadValue<short>(Pointers.Room, true);

        public void RefreshFlags()
        {
            ReadFlags(Pointers.DocumentFlags, Memory.DocumentFlags, 4);
            ReadFlags(Pointers.KilledFlags, Memory.KilledFlags, 64);
            ReadFlags(Pointers.EventFlags, Memory.EventFlags, 64);
            ReadFlags(Pointers.ItemFlags, Memory.ItemFlags, 64);
            ReadFlags(Pointers.MapFlags, Memory.MapFlags, 32);
        }

        private void ReadFlags(IntPtr flagPointer, FlagCollection list, int size)
        {
            IntPtr pointer = flagPointer;
            int offset = 0;

            if (list.Count <= 0)
            {
                for (int i = 0; i < (size / 4); ++i)
                {
                    int val = _process.ReadValue<int>(pointer, Emulator.IsBigEndian);

                    for (int b = 0; b < (8 * 4); ++b)
                    {
                        FlagModel model = new()
                        {
                            Bit = b,
                            Flag = list.Name,
                            FlagPointer = flagPointer,
                            Offset = offset++
                        };
                        model.Name = ReadFlagName(model);
                        model.SetValue(((val >> model.Bit) & 1) != 0, pointer);
                        list.Add(model);
                    }

                    pointer = IntPtr.Add(pointer, 4);
                }
            }
            else
            {
                foreach (FlagModel model in list)
                {
                    int val = _process.ReadValue<int>(pointer, Emulator.IsBigEndian);
                    model.SetValue(((val >> model.Bit) & 1) != 0, pointer);

                    if (model.Bit == 31)
                        pointer = IntPtr.Add(pointer, 4);
                }
            }
        }

        private static string ReadFlagName(FlagModel model) => 
            Program.Models.AppViewModel.FlagNames.GetValueOrDefault($"{model.Flag}.{model.Offset}");

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Emulator?.Dispose();
                    Emulator = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}