using System;
using System.Collections.Generic;
using System.IO;

namespace RDXplorer.Models.RDX
{
    public class DocumentModel : BaseNotifyModel
    {
        private FileInfo _pathInfo;
        public FileInfo PathInfo
        {
            get => _pathInfo;
            private set => SetField(ref _pathInfo, value);
        }

        private HeaderModel _header;
        public HeaderModel Header
        {
            get => _header;
            private set => SetField(ref _header, value);
        }

        private List<EnemyModel> _enemy;
        public List<EnemyModel> Enemy
        {
            get
            {
                if (_enemy == null)
                    SetField(ref _enemy, ReadEnemy(PathInfo.OpenRead(), Header));
                return _enemy;
            }

            private set => SetField(ref _enemy, value);
        }

        private List<PlayerModel> _player;
        public List<PlayerModel> Player
        {
            get
            {
                if (_player == null)
                    SetField(ref _player, ReadPlayer(PathInfo.OpenRead(), Header));
                return _player;
            }

            private set => SetField(ref _player, value);
        }

        private List<ItemModel> _item;
        public List<ItemModel> Item
        {
            get
            {
                if (_item == null)
                    SetField(ref _item, ReadItem(PathInfo.OpenRead(), Header));
                return _item;
            }

            private set => SetField(ref _item, value);
        }

        private List<ObjectModel> _object;
        public List<ObjectModel> Object
        {
            get
            {
                if (_object == null)
                    SetField(ref _object, ReadObject(PathInfo.OpenRead(), Header));
                return _object;
            }

            private set => SetField(ref _object, value);
        }

        private List<CameraModel> _camera;
        public List<CameraModel> Camera
        {
            get
            {
                if (_camera == null)
                    SetField(ref _camera, ReadCamera(PathInfo.OpenRead(), Header));
                return _camera;
            }

            private set => SetField(ref _camera, value);
        }

        public DocumentModel(FileInfo file)
        {
            PathInfo = file;
            LoadHeader();
        }

        public static HeaderModel ReadHeader(Stream stream)
        {
            HeaderModel header = new();

            using (BinaryReader br = new(stream))
            {
                header.Version.SetValue(stream.Position, br.ReadBytes(4));

                // TODO: Detect if file is in PRS format
                // TODO: Implement C# PRS library https://github.com/Sewer56/dlang-prs
                if (header.Version.Value == 1092616192 || header.Version.Value == 1074077368)
                {
                    stream.Seek(16, SeekOrigin.Begin);
                    header.Scripts.SetValue(stream.Position, br.ReadBytes(4));
                    header.Model.SetValue(stream.Position, br.ReadBytes(4));
                    header.Motion.SetValue(stream.Position, br.ReadBytes(4));
                    header.Flags.SetValue(stream.Position, br.ReadBytes(4));
                    header.Texture.SetValue(stream.Position, br.ReadBytes(4));

                    stream.Seek(96, SeekOrigin.Begin);
                    header.Author.SetValue(stream.Position, br.ReadBytes(32));

                    stream.Seek(header.Scripts.Value, SeekOrigin.Begin);
                    header.Camera.SetValue(stream.Position, br.ReadBytes(4));
                    header.Lighting.SetValue(stream.Position, br.ReadBytes(4));
                    header.Enemy.SetValue(stream.Position, br.ReadBytes(4));
                    header.Object.SetValue(stream.Position, br.ReadBytes(4));
                    header.Unknown1.SetValue(stream.Position, br.ReadBytes(4));
                    header.Unknown2.SetValue(stream.Position, br.ReadBytes(4));
                    header.SCA.SetValue(stream.Position, br.ReadBytes(4));
                    header.AOT.SetValue(stream.Position, br.ReadBytes(4));
                    header.Unknown3.SetValue(stream.Position, br.ReadBytes(4));
                    header.Player.SetValue(stream.Position, br.ReadBytes(4));
                    header.Unknown4.SetValue(stream.Position, br.ReadBytes(4));
                    header.Unknown5.SetValue(stream.Position, br.ReadBytes(4));
                    header.Unknown6.SetValue(stream.Position, br.ReadBytes(4));
                    header.Event.SetValue(stream.Position, br.ReadBytes(4));
                    header.Text.SetValue(stream.Position, br.ReadBytes(4));
                    header.Sysmes.SetValue(stream.Position, br.ReadBytes(4));

                    stream.Seek(256, SeekOrigin.Begin);
                    header.Camera.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Lighting.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Enemy.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Object.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Unknown1.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Unknown2.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.SCA.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.AOT.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Unknown3.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Player.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Unknown4.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Unknown5.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Unknown6.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Event.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Text.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Sysmes.Count.SetValue(stream.Position, br.ReadBytes(4));
                }
            }

            return header;
        }

        public static List<EnemyModel> ReadEnemy(Stream stream, HeaderModel header)
        {
            List<EnemyModel> list = new();

            using (BinaryReader br = new(stream))
            {
                stream.Seek(header.Enemy.Value, SeekOrigin.Begin);

                for (int i = 0; i < header.Enemy.Count.Value; i++)
                {
                    EnemyModel model = new();

                    model.Offset = (IntPtr)stream.Position;

                    model.Unknown0.SetValue(stream.Position, br.ReadBytes(4));
                    model.Type.SetValue(stream.Position, br.ReadBytes(2));
                    model.Unknown1.SetValue(stream.Position, br.ReadBytes(2));
                    model.Unknown2.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown3.SetValue(stream.Position, br.ReadBytes(1));
                    model.Slot.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown4.SetValue(stream.Position, br.ReadBytes(1));
                    model.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown5.SetValue(stream.Position, br.ReadBytes(4));
                    model.Rotation.SetValue(stream.Position, br.ReadBytes(2));
                    model.Unknown6.SetValue(stream.Position, br.ReadBytes(2));
                    model.Unknown7.SetValue(stream.Position, br.ReadBytes(4));

                    list.Add(model);
                }

                stream.Close();
            }

            return list;
        }

        public static List<PlayerModel> ReadPlayer(Stream stream, HeaderModel header)
        {
            List<PlayerModel> list = new();

            using (BinaryReader br = new(stream))
            {
                stream.Seek(header.Player.Value, SeekOrigin.Begin);

                for (int i = 0; i < header.Player.Count.Value; i++)
                {
                    PlayerModel model = new();

                    model.Offset = (IntPtr)stream.Position;

                    model.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Rotation.SetValue(stream.Position, br.ReadBytes(4));

                    list.Add(model);
                }

                stream.Close();
            }

            return list;
        }

        public static List<ItemModel> ReadItem(Stream stream, HeaderModel header)
        {
            List<ItemModel> list = new();

            using (BinaryReader br = new(stream))
            {
                stream.Seek(header.Unknown1.Value, SeekOrigin.Begin);

                for (int i = 0; i < header.Unknown1.Count.Value; i++)
                {
                    ItemModel model = new();

                    model.Offset = (IntPtr)stream.Position;

                    model.Unknown1.SetValue(stream.Position, br.ReadBytes(4));
                    model.Type.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown3.SetValue(stream.Position, br.ReadBytes(4));
                    model.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.XRot.SetValue(stream.Position, br.ReadBytes(2));
                    model.YRot.SetValue(stream.Position, br.ReadBytes(2));
                    model.ZRot.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown7.SetValue(stream.Position, br.ReadBytes(4));

                    list.Add(model);
                }
            }


            return list;
        }

        public static List<ObjectModel> ReadObject(Stream stream, HeaderModel header)
        {
            List<ObjectModel> list = new();

            using (BinaryReader br = new(stream))
            {
                stream.Seek(header.Object.Value, SeekOrigin.Begin);

                for (int i = 0; i < header.Object.Count.Value; i++)
                {
                    ObjectModel model = new();

                    model.Offset = (IntPtr)stream.Position;

                    model.Visible.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown1.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown2.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown3.SetValue(stream.Position, br.ReadBytes(1));
                    model.Type.SetValue(stream.Position, br.ReadBytes(2));
                    model.Unknown4.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown5.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown6.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown7.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown8.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown9.SetValue(stream.Position, br.ReadBytes(1));
                    model.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.XRot.SetValue(stream.Position, br.ReadBytes(2));
                    model.YRot.SetValue(stream.Position, br.ReadBytes(2));
                    model.ZRot.SetValue(stream.Position, br.ReadBytes(2));
                    model.Unknown10.SetValue(stream.Position, br.ReadBytes(2));
                    model.Unknown11.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown12.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown13.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown14.SetValue(stream.Position, br.ReadBytes(1));

                    list.Add(model);
                }
            }

            return list;
        }

        public static List<CameraModel> ReadCamera(Stream stream, HeaderModel header)
        {
            List<CameraModel> list = new();

            using (BinaryReader br = new(stream))
            {
                stream.Seek(header.Camera.Value, SeekOrigin.Begin);

                for (int i = 0; i < header.Camera.Count.Value; i++)
                {
                    CameraModel model = new();

                    model.Offset = (IntPtr)stream.Position;

                    model.Unknown1.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown2.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown3.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown4.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown5.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown6.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown7.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown8.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown9.SetValue(stream.Position, br.ReadBytes(4));

                    model.Unknown10.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown11.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown12.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown13.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown14.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown15.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown16.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown17.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown18.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown19.SetValue(stream.Position, br.ReadBytes(4));

                    model.Unknown20.SetValue(stream.Position, br.ReadBytes(0x268));

                    list.Add(model);
                }
            }

            return list;
        }

        public void LoadHeader() =>
            Header = ReadHeader(PathInfo.OpenRead());

        public void LoadEnemy() =>
            Enemy = ReadEnemy(PathInfo.OpenRead(), Header);

        public void LoadPlayer() =>
            Player = ReadPlayer(PathInfo.OpenRead(), Header);

        public void LoadItem() =>
            Item = ReadItem(PathInfo.OpenRead(), Header);

        public void LoadObject() =>
            Object = ReadObject(PathInfo.OpenRead(), Header);

        public void LoadCamera() =>
            Camera = ReadCamera(PathInfo.OpenRead(), Header);
    }
}