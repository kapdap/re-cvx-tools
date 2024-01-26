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

        private List<ModelTableModel> _model;
        public List<ModelTableModel> Model
        {
            get
            {
                if (_model == null)
                    SetField(ref _model, ReadModels(PathInfo, Header));
                return _model;
            }

            private set => SetField(ref _model, value);
        }

        private List<MotionTableModel> _motion;
        public List<MotionTableModel> Motion
        {
            get
            {
                if (_motion == null)
                    SetField(ref _motion, ReadMotions(PathInfo, Header));
                return _motion;
            }

            private set => SetField(ref _motion, value);
        }

        private List<ScriptModel> _script;
        public List<ScriptModel> Script
        {
            get
            {
                if (_script == null)
                    SetField(ref _script, ReadScripts(PathInfo, Header));
                return _script;
            }

            private set => SetField(ref _script, value);
        }

        private List<TextureTableModel> _texture;
        public List<TextureTableModel> Texture
        {
            get
            {
                if (_texture == null)
                    SetField(ref _texture, ReadTextures(PathInfo, Header));
                return _texture;
            }

            private set => SetField(ref _texture, value);
        }

        private List<CameraModel> _camera;
        public List<CameraModel> Camera
        {
            get
            {
                if (_camera == null)
                    SetField(ref _camera, ReadCamera(PathInfo, Header));
                return _camera;
            }

            private set => SetField(ref _camera, value);
        }

        private List<LightingModel> _lighting;
        public List<LightingModel> Lighting
        {
            get
            {
                if (_lighting == null)
                    SetField(ref _lighting, ReadLighting(PathInfo, Header));
                return _lighting;
            }

            private set => SetField(ref _lighting, value);
        }

        private List<ActorModel> _actor;
        public List<ActorModel> Actor
        {
            get
            {
                if (_actor == null)
                    SetField(ref _actor, ReadActor(PathInfo, Header));
                return _actor;
            }

            private set => SetField(ref _actor, value);
        }

        private List<ObjectModel> _object;
        public List<ObjectModel> Object
        {
            get
            {
                if (_object == null)
                    SetField(ref _object, ReadObject(PathInfo, Header));
                return _object;
            }

            private set => SetField(ref _object, value);
        }

        private List<ItemModel> _item;
        public List<ItemModel> Item
        {
            get
            {
                if (_item == null)
                    SetField(ref _item, ReadItem(PathInfo, Header));
                return _item;
            }

            private set => SetField(ref _item, value);
        }

        private List<EffectModel> _effect;
        public List<EffectModel> Effect
        {
            get
            {
                if (_effect == null)
                    SetField(ref _effect, ReadEffect(PathInfo, Header));
                return _effect;
            }

            private set => SetField(ref _effect, value);
        }

        private List<BoundaryModel> _boundary;
        public List<BoundaryModel> Boundary
        {
            get
            {
                if (_boundary == null)
                    SetField(ref _boundary, ReadBoundary(PathInfo, Header));
                return _boundary;
            }

            private set => SetField(ref _boundary, value);
        }

        private List<AOTModel> _aot;
        public List<AOTModel> AOT
        {
            get
            {
                if (_aot == null)
                    SetField(ref _aot, ReadAOT(PathInfo, Header));
                return _aot;
            }

            private set => SetField(ref _aot, value);
        }

        private List<TriggerModel> _trigger;
        public List<TriggerModel> Trigger
        {
            get
            {
                if (_trigger == null)
                    SetField(ref _trigger, ReadTrigger(PathInfo, Header));
                return _trigger;
            }

            private set => SetField(ref _trigger, value);
        }

        private List<PlayerModel> _player;
        public List<PlayerModel> Player
        {
            get
            {
                if (_player == null)
                    SetField(ref _player, ReadPlayer(PathInfo, Header));
                return _player;
            }

            private set => SetField(ref _player, value);
        }

        private List<EventModel> _event;
        public List<EventModel> Event
        {
            get
            {
                if (_event == null)
                    SetField(ref _event, ReadEvent(PathInfo, Header));
                return _event;
            }

            private set => SetField(ref _event, value);
        }

        public DocumentModel(FileInfo file)
        {
            PathInfo = file;
            Header = ReadHeader(file);
        }

        public static HeaderModel ReadHeader(FileInfo file)
        {
            HeaderModel header = new();

            using (Stream stream = file.OpenRead())
            {
                BinaryReader br = new(stream);

                header.Version.SetValue(stream.Position, br.ReadBytes(4));

                if (header.Version.Value == 0x41200000 ||
                    header.Version.Value == 0x40051EB8)
                {
                    stream.Seek(16, SeekOrigin.Begin);
                    header.Tables.SetValue(stream.Position, br.ReadBytes(4));
                    header.Model.SetValue(stream.Position, br.ReadBytes(4));
                    header.Motion.SetValue(stream.Position, br.ReadBytes(4));
                    header.Script.SetValue(stream.Position, br.ReadBytes(4));
                    header.Texture.SetValue(stream.Position, br.ReadBytes(4));

                    stream.Seek(96, SeekOrigin.Begin);
                    header.Author.SetValue(stream.Position, br.ReadBytes(32));

                    stream.Seek(header.Tables.Value, SeekOrigin.Begin);
                    header.Camera.SetValue(stream.Position, br.ReadBytes(4));
                    header.Lighting.SetValue(stream.Position, br.ReadBytes(4));
                    header.Actor.SetValue(stream.Position, br.ReadBytes(4));
                    header.Object.SetValue(stream.Position, br.ReadBytes(4));
                    header.Item.SetValue(stream.Position, br.ReadBytes(4));
                    header.Effect.SetValue(stream.Position, br.ReadBytes(4));
                    header.Boundary.SetValue(stream.Position, br.ReadBytes(4));
                    header.AOT.SetValue(stream.Position, br.ReadBytes(4));
                    header.Trigger.SetValue(stream.Position, br.ReadBytes(4));
                    header.Player.SetValue(stream.Position, br.ReadBytes(4));
                    header.Event.SetValue(stream.Position, br.ReadBytes(4));
                    header.Unknown1.SetValue(stream.Position, br.ReadBytes(4));
                    header.Unknown2.SetValue(stream.Position, br.ReadBytes(4));
                    header.Action.SetValue(stream.Position, br.ReadBytes(4));
                    header.Text.SetValue(stream.Position, br.ReadBytes(4));
                    header.Sysmes.SetValue(stream.Position, br.ReadBytes(4));

                    stream.Seek(256, SeekOrigin.Begin);
                    header.Camera.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Lighting.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Actor.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Object.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Item.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Effect.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Boundary.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.AOT.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Trigger.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Player.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Event.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Unknown1.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Unknown2.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Action.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Text.Count.SetValue(stream.Position, br.ReadBytes(4));
                    header.Sysmes.Count.SetValue(stream.Position, br.ReadBytes(4));

                    stream.Seek(header.Texture.Value, SeekOrigin.Begin);
                    header.Texture.Count.SetValue(stream.Position, br.ReadBytes(4));
                }
            }

            return header;
        }

        public static List<ModelTableModel> ReadModels(FileInfo file, HeaderModel header)
        {
            List<ModelTableModel> list = new();

            using (Stream stream = file.OpenRead())
            {
                BinaryReader br = new(stream);

                stream.Seek(header.Model.Value, SeekOrigin.Begin);

                for (int i = 0; i < 256; i++)
                {
                    ModelTableModel tableModel = new();

                    tableModel.Offset = (IntPtr)stream.Position;
                    tableModel.Pointer.SetValue(stream.Position, br.ReadBytes(4));

                    if (tableModel.Pointer.Value == 0 ||
                        tableModel.Pointer.Text == "SKIN" ||
                        tableModel.Pointer.Text.StartsWith("MDL"))
                        break;

                    list.Add(tableModel);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    uint nextOffset = i < list.Count - 1 ? list[i + 1].Pointer.Value : header.Motion.Value;

                    ModelTableModel tableModel = list[i];
                    tableModel.Size = nextOffset - tableModel.Pointer.Value;

                    stream.Seek(tableModel.Pointer.Value, SeekOrigin.Begin);

                    while (stream.Position < nextOffset)
                    {
                        ModelBlockModel blockModel = new();

                        blockModel.Table = tableModel;
                        blockModel.Offset = (IntPtr)stream.Position;

                        tableModel.Blocks.Add(blockModel);

                        DataEntryModel<uint> dataModelA = new();
                        dataModelA.SetValue(stream.Position, br.ReadBytes(4));

                        DataEntryModel<uint> dataModelB = new();
                        dataModelB.SetValue(stream.Position, br.ReadBytes(4));

                        if (dataModelA.Text == "SKIN" ||
                            dataModelA.Text.StartsWith("MDL"))
                        {
                            blockModel.Type = dataModelA;
                        }
                        else
                        {
                            blockModel.Type = dataModelB;
                            blockModel.Size = dataModelA;
                            blockModel.HasSize = true;
                        }

                        if (blockModel.HasSize)
                        {
                            stream.Seek(blockModel.Size.Value, SeekOrigin.Current);

                            while (stream.Position < nextOffset)
                            {
                                DataEntryModel<uint> dataModelV = new();
                                dataModelV.SetValue(stream.Position, br.ReadBytes(4));

                                if (dataModelV.Text == "SKIN" ||
                                    dataModelV.Text == "MASK" ||
                                    dataModelV.Text.StartsWith("MDL"))
                                {
                                    stream.Seek(-8, SeekOrigin.Current);
                                    break;
                                }
                            }
                        }
                        else
                            break;
                    }
                }
            }

            return list;
        }

        public static List<MotionTableModel> ReadMotions(FileInfo file, HeaderModel header)
        {
            List<MotionTableModel> list = new();

            using (Stream stream = file.OpenRead())
            {
                BinaryReader br = new(stream);

                stream.Seek(header.Motion.Value, SeekOrigin.Begin);

                uint firstPosition = 0;

                while (stream.Position < (firstPosition > 0 ? firstPosition : header.Script.Value))
                {
                    MotionTableModel tableModel = new();

                    tableModel.Offset = (IntPtr)stream.Position;
                    tableModel.Pointer.SetValue(stream.Position, br.ReadBytes(4));

                    if (tableModel.Pointer.Value > header.Script.Value)
                        break;

                    if (firstPosition == 0)
                        firstPosition = tableModel.Pointer.Value;

                    if (tableModel.Pointer.Value != 0)
                        list.Add(tableModel);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    MotionTableModel tableModel = list[i];

                    if (tableModel.Pointer.Value == 0)
                        continue;

                    stream.Seek(tableModel.Pointer.Value, SeekOrigin.Begin);

                    uint nextOffset = i < list.Count - 1 ? list[i + 1].Pointer.Value : header.Script.Value;

                    while (stream.Position < nextOffset)
                    {
                        MotionBlockModel blockModel = new();

                        blockModel.Table = tableModel;
                        blockModel.Offset = (IntPtr)stream.Position;

                        tableModel.Blocks.Add(blockModel);

                        blockModel.Size.SetValue(stream.Position, br.ReadBytes(4));
                        blockModel.Type.SetValue(stream.Position, br.ReadBytes(4));

                        stream.Seek(-4, SeekOrigin.Current);

                        blockModel.Data.SetValue(stream.Position, br.ReadBytes((int)blockModel.Size.Value));

                        while (stream.Position < nextOffset)
                        {
                            DataEntryModel<uint> dataModel = new();
                            dataModel.SetValue(stream.Position, br.ReadBytes(4));

                            if (dataModel.Value != 0 && dataModel.Value != 0xFFFFFFFF)
                            {
                                stream.Seek(-4, SeekOrigin.Current);
                                break;
                            }
                        }
                    }
                }
            }

            return list;
        }

        public static List<ScriptModel> ReadScripts(FileInfo file, HeaderModel header)
        {
            List<ScriptModel> list = new();

            using (Stream stream = file.OpenRead())
            {
                BinaryReader br = new(stream);

                stream.Seek(header.Script.Value, SeekOrigin.Begin);

                while (stream.Position < (list.Count > 0 ? list[0].Pointer.Value : stream.Position + 1))
                {
                    ScriptModel model = new();

                    model.Offset = (IntPtr)stream.Position;

                    model.Position.SetValue(stream.Position, br.ReadBytes(4));
                    model.Pointer.SetValue(model.Offset, BitConverter.GetBytes(header.Script.Value + model.Position.Value));

                    list.Add(model);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    uint nextOffset = i < list.Count - 1 ? list[i + 1].Pointer.Value : header.Texture.Value;

                    ScriptModel model = list[i];

                    stream.Seek(model.Pointer.Value, SeekOrigin.Begin);

                    model.Size = nextOffset - model.Pointer.Value;
                    model.Data.SetValue(stream.Position, br.ReadBytes((int)model.Size));
                }
            }

            return list;
        }

        public static List<TextureTableModel> ReadTextures(FileInfo file, HeaderModel header)
        {
            List<TextureTableModel> list = new();

            using (Stream stream = file.OpenRead())
            {
                BinaryReader br = new(stream);

                stream.Seek(header.Texture.Value + 4, SeekOrigin.Begin);

                for (int i = 0; i < header.Texture.Count.Value; i++)
                {
                    TextureTableModel model = new();

                    model.Offset = (IntPtr)stream.Position;
                    model.Pointer.SetValue(stream.Position, br.ReadBytes(4));

                    if (model.Pointer.Value == 0)
                        break;

                    list.Add(model);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    uint nextOffset = i < list.Count - 1 ? list[i + 1].Pointer.Value : (uint)file.Length;

                    TextureTableModel tableModel = list[i];
                    tableModel.Size = nextOffset - tableModel.Pointer.Value;

                    stream.Seek(tableModel.Pointer.Value, SeekOrigin.Begin);

                    while (stream.Position < nextOffset)
                    {
                        TextureBlockModel blockModel = new();

                        blockModel.Offset = (IntPtr)stream.Position;
                        blockModel.Table = tableModel;

                        blockModel.Type.SetValue(stream.Position, br.ReadBytes(4));

                        if (blockModel.Type.Value == 0xFFFFFFFF)
                            break;

                        blockModel.Size.SetValue(stream.Position, br.ReadBytes(4));

                        tableModel.Blocks.Add(blockModel);

                        stream.Seek(24, SeekOrigin.Current);
                        stream.Seek(blockModel.Size.Value, SeekOrigin.Current);
                    }
                }
            }

            return list;
        }

        public static List<CameraModel> ReadCamera(FileInfo file, HeaderModel header)
        {
            List<CameraModel> list = new();

            using (Stream stream = file.OpenRead())
            {
                BinaryReader br = new(stream);

                stream.Seek(header.Camera.Value, SeekOrigin.Begin);

                for (int i = 0; i < header.Camera.Count.Value; i++)
                {
                    CameraModel model = new();

                    model.Offset = (IntPtr)stream.Position;

                    model.Unknown1.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown2.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown3.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown4.SetValue(stream.Position, br.ReadBytes(1));
                    model.Pointer.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown6.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown7.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown8.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown9.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown10.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown11.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown12.SetValue(stream.Position, br.ReadBytes(4));
                    model.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown16.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown17.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown18.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown19.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown20.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown21.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown22.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown23.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown24.SetValue(stream.Position, br.ReadBytes(4));
                    model.XRotation.SetValue(stream.Position, br.ReadBytes(4));
                    model.YRotation.SetValue(stream.Position, br.ReadBytes(4));
                    model.ZRotation.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown28.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown29.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown30.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown31.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown32.SetValue(stream.Position, br.ReadBytes(4));
                    model.Perspective.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown34.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown35.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown36.SetValue(stream.Position, br.ReadBytes(0x228));

                    list.Add(model);
                }
            }

            return list;
        }

        public static List<LightingModel> ReadLighting(FileInfo file, HeaderModel header)
        {
            List<LightingModel> list = new();

            using (Stream stream = file.OpenRead())
            {
                BinaryReader br = new(stream);

                stream.Seek(header.Lighting.Value, SeekOrigin.Begin);

                for (int i = 0; i < header.Lighting.Count.Value; i++)
                {
                    LightingModel model = new();

                    model.Offset = (IntPtr)stream.Position;

                    model.Unknown1.SetValue(stream.Position, br.ReadBytes(2));
                    model.Unknown2.SetValue(stream.Position, br.ReadBytes(2));
                    model.Unknown3.SetValue(stream.Position, br.ReadBytes(4));

                    model.Unknown4.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown5.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown6.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown7.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown8.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown9.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown10.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown11.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown12.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown13.SetValue(stream.Position, br.ReadBytes(4));

                    model.Data.SetValue(stream.Position, br.ReadBytes(0xB0));

                    list.Add(model);
                }
            }

            return list;
        }

        public static List<ActorModel> ReadActor(FileInfo file, HeaderModel header)
        {
            List<ActorModel> list = new();

            using (Stream stream = file.OpenRead())
            {
                BinaryReader br = new(stream);

                stream.Seek(header.Actor.Value, SeekOrigin.Begin);

                for (int i = 0; i < header.Actor.Count.Value; i++)
                {
                    ActorModel model = new();

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

        public static List<ObjectModel> ReadObject(FileInfo file, HeaderModel header)
        {
            List<ObjectModel> list = new();

            using (Stream stream = file.OpenRead())
            {
                BinaryReader br = new(stream);

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

        public static List<ItemModel> ReadItem(FileInfo file, HeaderModel header)
        {
            List<ItemModel> list = new();

            using (Stream stream = file.OpenRead())
            {
                BinaryReader br = new(stream);

                stream.Seek(header.Item.Value, SeekOrigin.Begin);

                for (int i = 0; i < header.Item.Count.Value; i++)
                {
                    ItemModel model = new();

                    model.Offset = (IntPtr)stream.Position;

                    model.Unknown1.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown2.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown3.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown4.SetValue(stream.Position, br.ReadBytes(1));
                    model.Type.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown5.SetValue(stream.Position, br.ReadBytes(4));
                    model.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.XRot.SetValue(stream.Position, br.ReadBytes(2));
                    model.YRot.SetValue(stream.Position, br.ReadBytes(2));
                    model.ZRot.SetValue(stream.Position, br.ReadBytes(2));
                    model.Unknown6.SetValue(stream.Position, br.ReadBytes(2));
                    model.Unknown7.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown8.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown9.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown10.SetValue(stream.Position, br.ReadBytes(1));

                    list.Add(model);
                }
            }


            return list;
        }

        public static List<EffectModel> ReadEffect(FileInfo file, HeaderModel header)
        {
            List<EffectModel> list = new();

            using (Stream stream = file.OpenRead())
            {
                BinaryReader br = new(stream);

                stream.Seek(header.Effect.Value, SeekOrigin.Begin);

                for (int i = 0; i < header.Effect.Count.Value; i++)
                {
                    EffectModel model = new();

                    model.Offset = (IntPtr)stream.Position;

                    model.Unknown1.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown2.SetValue(stream.Position, br.ReadBytes(2));
                    model.Unknown3.SetValue(stream.Position, br.ReadBytes(2));
                    model.Unknown4.SetValue(stream.Position, br.ReadBytes(4));
                    model.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Width.SetValue(stream.Position, br.ReadBytes(4));
                    model.Height.SetValue(stream.Position, br.ReadBytes(4));
                    model.Length.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown11.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown12.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown13.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown14.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown15.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown16.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown17.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown18.SetValue(stream.Position, br.ReadBytes(4));

                    list.Add(model);
                }
            }

            return list;
        }

        public static List<BoundaryModel> ReadBoundary(FileInfo file, HeaderModel header)
        {
            List<BoundaryModel> list = new();

            using (Stream stream = file.OpenRead())
            {
                BinaryReader br = new(stream);

                stream.Seek(header.Boundary.Value, SeekOrigin.Begin);

                for (int i = 0; i < header.Boundary.Count.Value; i++)
                {
                    BoundaryModel model = new();

                    model.Offset = (IntPtr)stream.Position;

                    model.Unknown1.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown2.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown3.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown4.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown5.SetValue(stream.Position, br.ReadBytes(4));
                    model.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Width.SetValue(stream.Position, br.ReadBytes(4));
                    model.Height.SetValue(stream.Position, br.ReadBytes(4));
                    model.Length.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown12.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown13.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown14.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown15.SetValue(stream.Position, br.ReadBytes(1));

                    list.Add(model);
                }
            }

            return list;
        }

        public static List<AOTModel> ReadAOT(FileInfo file, HeaderModel header)
        {
            List<AOTModel> list = new();

            using (Stream stream = file.OpenRead())
            {
                BinaryReader br = new(stream);

                stream.Seek(header.AOT.Value, SeekOrigin.Begin);

                for (int i = 0; i < header.AOT.Count.Value; i++)
                {
                    AOTModel model = new();

                    model.Offset = (IntPtr)stream.Position;

                    model.Unknown1.SetValue(stream.Position, br.ReadBytes(1));
                    model.Type.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown3.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown4.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown5.SetValue(stream.Position, br.ReadBytes(4));
                    model.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Width.SetValue(stream.Position, br.ReadBytes(4));
                    model.Height.SetValue(stream.Position, br.ReadBytes(4));
                    model.Length.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown12.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown13.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown14.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown15.SetValue(stream.Position, br.ReadBytes(1));

                    list.Add(model);
                }
            }

            return list;
        }

        public static List<TriggerModel> ReadTrigger(FileInfo file, HeaderModel header)
        {
            List<TriggerModel> list = new();

            using (Stream stream = file.OpenRead())
            {
                BinaryReader br = new(stream);

                stream.Seek(header.Trigger.Value, SeekOrigin.Begin);

                for (int i = 0; i < header.Trigger.Count.Value; i++)
                {
                    TriggerModel model = new();

                    model.Offset = (IntPtr)stream.Position;

                    model.Unknown1.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown2.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown3.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown4.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown5.SetValue(stream.Position, br.ReadBytes(4));
                    model.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Width.SetValue(stream.Position, br.ReadBytes(4));
                    model.Height.SetValue(stream.Position, br.ReadBytes(4));
                    model.Length.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown12.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown13.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown14.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown15.SetValue(stream.Position, br.ReadBytes(1));

                    list.Add(model);
                }
            }

            return list;
        }

        public static List<PlayerModel> ReadPlayer(FileInfo file, HeaderModel header)
        {
            List<PlayerModel> list = new();

            using (Stream stream = file.OpenRead())
            {
                BinaryReader br = new(stream);

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

        public static List<EventModel> ReadEvent(FileInfo file, HeaderModel header)
        {
            List<EventModel> list = new();

            using (Stream stream = file.OpenRead())
            {
                BinaryReader br = new(stream);

                stream.Seek(header.Event.Value, SeekOrigin.Begin);

                for (int i = 0; i < header.Event.Count.Value; i++)
                {
                    EventModel model = new();

                    model.Offset = (IntPtr)stream.Position;

                    model.Unknown1.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown2.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown3.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown4.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown5.SetValue(stream.Position, br.ReadBytes(4));
                    model.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Width.SetValue(stream.Position, br.ReadBytes(4));
                    model.Height.SetValue(stream.Position, br.ReadBytes(4));
                    model.Length.SetValue(stream.Position, br.ReadBytes(4));
                    model.Unknown12.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown13.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown14.SetValue(stream.Position, br.ReadBytes(1));
                    model.Unknown15.SetValue(stream.Position, br.ReadBytes(1));

                    list.Add(model);
                }
            }

            return list;
        }
    }
}