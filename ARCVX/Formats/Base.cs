using ARCVX.Reader;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ARCVX.Formats
{
    public class Base<T> : IBase<T>, IDisposable
        where T : struct
    {
        public virtual int MAGIC => throw new NotImplementedException("MAGIC has not been implemented");
        public virtual int MAGIC_LE => throw new NotImplementedException("MAGIC_LE has not been implemented");

        private int? _HEADER_SIZE;
        public int HEADER_SIZE
        {
            get
            {
                _HEADER_SIZE ??= Marshal.SizeOf<T>();
                return (int)_HEADER_SIZE;
            }
        }

        public FileInfo File { get; }
        public Stream Stream { get; private set; }
        public EndianReader Reader { get; private set; }

        private bool? _isValid = null;
        public virtual bool IsValid
        {
            get
            {
                _isValid ??= File.Exists &&
                    File.Length > HEADER_SIZE &&
                    (Magic == MAGIC || Magic == MAGIC_LE);

                return (bool)_isValid;
            }
        }

        private int? _magic;
        public virtual int Magic
        {
            get
            {
                _magic ??= GetMagic();
                return (int)_magic;
            }
        }

        private T? _header;
        public virtual T Header
        {
            get
            {
                _header ??= GetHeader();
                return (T)_header;
            }
        }

        public Base(FileInfo file) =>
            File = file;

        public Base(FileInfo file, Stream stream)
        {
            File = file;
            Stream = stream;
        }

        public virtual int GetMagic()
        {
            OpenReader();

            long position = Reader.GetPosition();
            Reader.SetPosition(0);

            int magic = Reader.ReadInt32(ByteOrder.LittleEndian);
            Reader.SetPosition(position);

            Reader.ByteOrder = magic == MAGIC ? ByteOrder.BigEndian : ByteOrder.LittleEndian;

            return magic;
        }

        public virtual T GetHeader() =>
            throw new NotImplementedException("GetHeader has not been implemented");

        public virtual void OpenReader()
        {
            if (Stream == null && File != null)
                Stream = File.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            if (Reader == null && Stream != null)
                Reader = new(Stream);
        }

        public virtual void CloseReader()
        {
            Reader?.Close();
            Stream?.Close();
        }

        public virtual void Dispose()
        {
            if (Reader != null)
                ((IDisposable)Reader).Dispose();

            if (Stream != null)
                ((IDisposable)Stream).Dispose();
        }
    }

    public interface IBase
    {
        int MAGIC { get; }
        int MAGIC_LE { get; }

        int HEADER_SIZE { get; }

        FileInfo File { get; }
        Stream Stream { get; }
        EndianReader Reader { get; }

        bool IsValid { get; }
        int Magic { get; }

        int GetMagic();

        void OpenReader();
        void CloseReader();
    }

    public interface IBase<T> : IBase
        where T : struct
    {
        T Header { get; }
        T GetHeader();
    }
}