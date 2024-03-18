// SPDX-FileCopyrightText: 2024 Kapdap <kapdap@pm.me>
//
// SPDX-License-Identifier: MIT
/*  ARCVX
 *  
 *  Copyright 2024 Kapdap <kapdap@pm.me>
 *
 *  Use of this source code is governed by an MIT-style
 *  license that can be found in the LICENSE file or at
 *  https://opensource.org/licenses/MIT.
 */

using ARCVX.Reader;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ARCVX.Formats
{
    public class Base : IBase, IDisposable
    {
        public FileInfo File { get; }
        public Stream Stream { get; private set; }
        public EndianReader Reader { get; private set; }

        public ByteOrder ByteOrder
        {
            get => Reader.ByteOrder;
            set => Reader.ByteOrder = value;
        }

        public Base(FileInfo file) =>
            File = file;

        public Base(FileInfo file, Stream stream)
        {
            File = file;
            Stream = stream;
        }

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

    public class Base<T> : Base, IBase<T>
        where T : struct
    {
        public virtual int MAGIC => throw new NotImplementedException("MAGIC has not been implemented");
        public virtual int MAGIC_LE => throw new NotImplementedException("MAGIC_LE has not been implemented");

        protected int? _HEADER_SIZE;
        public int HEADER_SIZE
        {
            get
            {
                _HEADER_SIZE ??= Marshal.SizeOf<T>();
                return (int)_HEADER_SIZE;
            }
        }

        protected bool? _isValid = null;
        public virtual bool IsValid
        {
            get
            {
                if (_isValid == null)
                    File.Refresh();

                _isValid ??= File.Exists &&
                    File.Length > HEADER_SIZE &&
                    (Magic == MAGIC || Magic == MAGIC_LE);

                return (bool)_isValid;
            }
        }

        protected int? _magic;
        public virtual int Magic
        {
            get
            {
                _magic ??= GetMagic();
                return (int)_magic;
            }
        }

        protected T? _header;
        public virtual T Header
        {
            get
            {
                if (IsValid)
                    _header ??= GetHeader();
                return (T)_header;
            }
        }

        public Base(FileInfo file) : base(file) { }
        public Base(FileInfo file, Stream stream) : base(file, stream) { }

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
    }

    public interface IBase
    {
        FileInfo File { get; }
        Stream Stream { get; }
        EndianReader Reader { get; }

        void OpenReader();
        void CloseReader();
    }

    public interface IBase<T> : IBase
        where T : struct
    {
        int MAGIC { get; }
        int MAGIC_LE { get; }

        int HEADER_SIZE { get; }

        bool IsValid { get; }

        int Magic { get; }
        T Header { get; }

        int GetMagic();
        T GetHeader();
    }
}