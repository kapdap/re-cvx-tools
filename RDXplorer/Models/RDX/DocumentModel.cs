using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RDXplorer.Models.RDX
{
    public class DocumentModel : BaseNotifyModel
    {
        private FileInfo _pathInfo;
        public FileInfo PathInfo
        {
            get
            {
                return _pathInfo;
            }

            private set
            {
                _pathInfo = value;
                OnPropertyChanged();
            }
        }

        private HeaderModel _header;
        public HeaderModel Header
        {
            get
            {
                return _header;
            }

            private set
            {
                _header = value;
                OnPropertyChanged();
            }
        }

        private List<EnemyModel> _enemy;
        public List<EnemyModel> Enemy
        {
            get
            {
                if (_enemy == null)
                    _enemy = ReadEnemy(PathInfo.OpenRead(), Header);

                return _enemy;
            }

            private set
            {
                _enemy = value;
                OnPropertyChanged();
            }
        }

        private List<PlayerModel> _player;
        public List<PlayerModel> Player
        {
            get
            {
                if (_player == null)
                    _player = ReadPlayer(PathInfo.OpenRead(), Header);

                return _player;
            }

            private set
            {
                _player = value;
                OnPropertyChanged();
            }
        }

        public DocumentModel(FileInfo file)
        {
            PathInfo = file;
            LoadHeader();
        }

        public static HeaderModel ReadHeader(Stream stream)
        {
            HeaderModel header = new HeaderModel();

            using (BinaryReader br = new BinaryReader(stream))
            {
                SetHeaderEntry(header.Version, stream.Position, br.ReadBytes(4), false);

                // TODO: Throw Error if file is in PRS compression format
                if (header.Version.Value == 1092616192)
                {
                    stream.Seek(16, SeekOrigin.Begin);
                    SetHeaderEntry(header.Scripts, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.Model, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.Motion, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.Flags, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.Texture, stream.Position, br.ReadBytes(4));

                    stream.Seek(96, SeekOrigin.Begin);
                    header.Author.Offset = stream.Position;
                    header.Author.Data = br.ReadBytes(32);
                    header.Author.Value = BitConverter.ToUInt32(header.Author.Data, 0);
                    header.Author.Text = Encoding.ASCII.GetString(header.Author.Data);
                    header.Author.IsPointer = false;

                    stream.Seek(header.Scripts.Value, SeekOrigin.Begin);
                    SetHeaderEntry(header.Camera, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.Lighting, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.Enemy, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.Object, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.Unknown1, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.Unknown2, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.SCA, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.AOT, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.Unknown3, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.Player, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.Unknown4, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.Unknown5, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.Unknown6, stream.Position, br.ReadBytes(4), false);
                    SetHeaderEntry(header.Event, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.Text, stream.Position, br.ReadBytes(4));
                    SetHeaderEntry(header.Sysmes, stream.Position, br.ReadBytes(4));

                    stream.Seek(256, SeekOrigin.Begin);
                    SetDataEntry(header.Camera.Count, stream.Position, br.ReadBytes(4));
                    header.Camera.Count.Value = BitConverter.ToUInt32(header.Camera.Count.Data, 0);

                    SetDataEntry(header.Lighting.Count, stream.Position, br.ReadBytes(4));
                    header.Lighting.Count.Value = BitConverter.ToUInt32(header.Lighting.Count.Data, 0);

                    SetDataEntry(header.Enemy.Count, stream.Position, br.ReadBytes(4));
                    header.Enemy.Count.Value = BitConverter.ToUInt32(header.Enemy.Count.Data, 0);

                    SetDataEntry(header.Object.Count, stream.Position, br.ReadBytes(4));
                    header.Object.Count.Value = BitConverter.ToUInt32(header.Object.Count.Data, 0);

                    SetDataEntry(header.Unknown1.Count, stream.Position, br.ReadBytes(4));
                    header.Unknown1.Count.Value = BitConverter.ToUInt32(header.Unknown1.Count.Data, 0);

                    SetDataEntry(header.Unknown2.Count, stream.Position, br.ReadBytes(4));
                    header.Unknown2.Count.Value = BitConverter.ToUInt32(header.Unknown2.Count.Data, 0);

                    SetDataEntry(header.SCA.Count, stream.Position, br.ReadBytes(4));
                    header.SCA.Count.Value = BitConverter.ToUInt32(header.SCA.Count.Data, 0);

                    SetDataEntry(header.AOT.Count, stream.Position, br.ReadBytes(4));
                    header.AOT.Count.Value = BitConverter.ToUInt32(header.AOT.Count.Data, 0);

                    SetDataEntry(header.Unknown3.Count, stream.Position, br.ReadBytes(4));
                    header.Unknown3.Count.Value = BitConverter.ToUInt32(header.Unknown3.Count.Data, 0);

                    SetDataEntry(header.Player.Count, stream.Position, br.ReadBytes(4));
                    header.Player.Count.Value = BitConverter.ToUInt32(header.Player.Count.Data, 0);

                    SetDataEntry(header.Unknown4.Count, stream.Position, br.ReadBytes(4));
                    header.Unknown4.Count.Value = BitConverter.ToUInt32(header.Unknown4.Count.Data, 0);

                    SetDataEntry(header.Unknown5.Count, stream.Position, br.ReadBytes(4));
                    header.Unknown5.Count.Value = BitConverter.ToUInt32(header.Unknown5.Count.Data, 0);

                    SetDataEntry(header.Unknown6.Count, stream.Position, br.ReadBytes(4));
                    header.Unknown6.Count.Value = BitConverter.ToUInt32(header.Unknown6.Count.Data, 0);

                    SetDataEntry(header.Event.Count, stream.Position, br.ReadBytes(4));
                    header.Event.Count.Value = BitConverter.ToUInt32(header.Event.Count.Data, 0);

                    SetDataEntry(header.Text.Count, stream.Position, br.ReadBytes(4));
                    header.Text.Count.Value = BitConverter.ToUInt32(header.Text.Count.Data, 0);

                    SetDataEntry(header.Sysmes.Count, stream.Position, br.ReadBytes(4));
                    header.Sysmes.Count.Value = BitConverter.ToUInt32(header.Sysmes.Count.Data, 0);
                }
            }

            return header;
        }

        public static List<EnemyModel> ReadEnemy(Stream stream, HeaderModel header)
        {
            List<EnemyModel> enemy = new List<EnemyModel>();

            if (header.Enemy.Count.Value <= 0)
                return enemy;

            using (BinaryReader br = new BinaryReader(stream))
            {
                stream.Seek(header.Enemy.Value, SeekOrigin.Begin);

                for (int i = 0; i < header.Enemy.Count.Value; i++)
                {
                    EnemyModel entry = new EnemyModel();

                    entry.Offset = stream.Position;

                    SetDataEntry(entry.Unknown0, stream.Position, br.ReadBytes(4));
                    entry.Unknown0.Value = BitConverter.ToInt32(entry.Unknown0.Data, 0);

                    SetDataEntry(entry.Type, stream.Position, br.ReadBytes(2));
                    entry.Type.Value = BitConverter.ToInt16(entry.Type.Data, 0);

                    SetDataEntry(entry.Unknown1, stream.Position, br.ReadBytes(2));
                    entry.Unknown1.Value = BitConverter.ToInt16(entry.Unknown1.Data, 0);

                    SetDataEntry(entry.Unknown2, stream.Position, br.ReadBytes(1));
                    entry.Unknown2.Value = entry.Unknown2.Data[0];

                    SetDataEntry(entry.Unknown3, stream.Position, br.ReadBytes(1));
                    entry.Unknown3.Value = entry.Unknown3.Data[0];

                    SetDataEntry(entry.Slot, stream.Position, br.ReadBytes(1));
                    entry.Slot.Value = entry.Slot.Data[0];

                    SetDataEntry(entry.Unknown4, stream.Position, br.ReadBytes(1));
                    entry.Unknown4.Value = entry.Unknown4.Data[0];

                    SetDataEntry(entry.X, stream.Position, br.ReadBytes(4));
                    entry.X.Value = BitConverter.ToSingle(entry.X.Data, 0);

                    SetDataEntry(entry.Y, stream.Position, br.ReadBytes(4));
                    entry.Y.Value = BitConverter.ToSingle(entry.Y.Data, 0);

                    SetDataEntry(entry.Z, stream.Position, br.ReadBytes(4));
                    entry.Z.Value = BitConverter.ToSingle(entry.Z.Data, 0);

                    SetDataEntry(entry.Unknown5, stream.Position, br.ReadBytes(4));
                    entry.Unknown5.Value = BitConverter.ToInt32(entry.Unknown5.Data, 0);

                    SetDataEntry(entry.Rotation, stream.Position, br.ReadBytes(2));
                    entry.Rotation.Value = BitConverter.ToInt16(entry.Rotation.Data, 0);

                    SetDataEntry(entry.Unknown6, stream.Position, br.ReadBytes(2));
                    entry.Unknown6.Value = BitConverter.ToInt16(entry.Unknown6.Data, 0);

                    SetDataEntry(entry.Unknown7, stream.Position, br.ReadBytes(4));
                    entry.Unknown7.Value = BitConverter.ToInt32(entry.Unknown7.Data, 0);

                    enemy.Add(entry);
                }
            }

            return enemy;
        }

        public static List<PlayerModel> ReadPlayer(Stream stream, HeaderModel header)
        {
            List<PlayerModel> player = new List<PlayerModel>();

            if (header.Player.Count.Value <= 0)
                return player;

            using (BinaryReader br = new BinaryReader(stream))
            {
                stream.Seek(header.Player.Value, SeekOrigin.Begin);

                for (int i = 0; i < header.Player.Count.Value; i++)
                {
                    PlayerModel entry = new PlayerModel();

                    entry.Offset = stream.Position;

                    SetDataEntry(entry.X, stream.Position, br.ReadBytes(4));
                    entry.X.Value = BitConverter.ToSingle(entry.X.Data, 0);

                    SetDataEntry(entry.Y, stream.Position, br.ReadBytes(4));
                    entry.Y.Value = BitConverter.ToSingle(entry.Y.Data, 0);

                    SetDataEntry(entry.Z, stream.Position, br.ReadBytes(4));
                    entry.Z.Value = BitConverter.ToSingle(entry.Z.Data, 0);

                    SetDataEntry(entry.Rotation, stream.Position, br.ReadBytes(4));
                    entry.Rotation.Value = BitConverter.ToInt32(entry.Rotation.Data, 0);

                    player.Add(entry);
                }
            }

            return player;
        }

        public void LoadHeader() =>
            Header = ReadHeader(PathInfo.OpenRead());

        public void LoadEnemy() =>
            Enemy = ReadEnemy(PathInfo.OpenRead(), Header);

        public void LoadPlayer() =>
            Player = ReadPlayer(PathInfo.OpenRead(), Header);

        public static void SetDataEntry<T>(DataEntryModel<T> model, long offset, byte[] data, bool isPointer = false)
        {
            model.Offset = offset;
            model.Data = data;
            model.Text = Encoding.ASCII.GetString(data);
            model.IsPointer = isPointer;
        }

        public static void SetHeaderEntry(HeaderEntryModel model, long offset, byte[] data, bool isPointer = true)
        {
            model.Offset = offset;
            model.Data = data;
            model.Value = BitConverter.ToUInt32(model.Data, 0);
            model.Text = Encoding.ASCII.GetString(data);
            model.IsPointer = isPointer;
        }
    }
}