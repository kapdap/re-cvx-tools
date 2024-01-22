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

        private List<ModelHeaderModel> _model;
        public List<ModelHeaderModel> Model
        {
            get
            {
                if (_model == null)
                    SetField(ref _model, ReadModels(PathInfo.OpenRead(), Header));
                return _model;
            }

            private set => SetField(ref _model, value);
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

        private List<LightingModel> _lighting;
        public List<LightingModel> Lighting
        {
            get
            {
                if (_lighting == null)
                    SetField(ref _lighting, ReadLighting(PathInfo.OpenRead(), Header));
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
                    SetField(ref _actor, ReadActor(PathInfo.OpenRead(), Header));
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
                    SetField(ref _object, ReadObject(PathInfo.OpenRead(), Header));
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
                    SetField(ref _item, ReadItem(PathInfo.OpenRead(), Header));
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
                    SetField(ref _effect, ReadEffect(PathInfo.OpenRead(), Header));
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
                    SetField(ref _boundary, ReadBoundary(PathInfo.OpenRead(), Header));
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
                    SetField(ref _aot, ReadAOT(PathInfo.OpenRead(), Header));
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
                    SetField(ref _trigger, ReadTrigger(PathInfo.OpenRead(), Header));
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
                    SetField(ref _player, ReadPlayer(PathInfo.OpenRead(), Header));
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
                    SetField(ref _event, ReadEvent(PathInfo.OpenRead(), Header));
                return _event;
            }

            private set => SetField(ref _event, value);
        }

        public DocumentModel(FileInfo file)
        {
            PathInfo = file;
            Header = ReadHeader(file.OpenRead());
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
                }
            }

            return header;
        }

        public static List<ModelHeaderModel> ReadModels(Stream stream, HeaderModel header)
        {
            List<ModelHeaderModel> list = new();

            using (BinaryReader br = new(stream))
            {
                stream.Seek(header.Model.Value, SeekOrigin.Begin);

                for (int i = 0; i < 256; i++)
                {
                    ModelHeaderModel model = new();

                    model.Offset = (IntPtr)stream.Position;
                    model.Pointer.SetValue(stream.Position, br.ReadBytes(4));

                    if (model.Pointer.Value == 0 ||
                        model.Pointer.Text == "SKIN" ||
                        model.Pointer.Text.StartsWith("MDL"))
                        break;

                    list.Add(model);
                }

                // TODO: Read all model sections (MDL, SKIN, BONE etc)
                for (int i = 0; i < list.Count; i++)
                {
                    ModelHeaderModel headerModel = list[i];
                    headerModel.Size = (i < list.Count - 1 ? list[i + 1].Pointer.Value : (uint)header.Motion.Offset) - headerModel.Pointer.Value;

                    stream.Seek(headerModel.Pointer.Value, SeekOrigin.Begin);

                    ModelEntryModel entryModel = new();
                    entryModel.Offset = (IntPtr)stream.Position;

                    DataEntryModel<uint> dataModelA = new();
                    dataModelA.SetValue(stream.Position, br.ReadBytes(4));

                    DataEntryModel<uint> dataModelB = new();
                    dataModelB.SetValue(stream.Position, br.ReadBytes(4));

                    if (dataModelA.Text == "SKIN" || dataModelA.Text.StartsWith("MDL"))
                    {
                        entryModel.Type = dataModelA;
                    }
                    else
                    {
                        entryModel.Type = dataModelB;
                        entryModel.Size = dataModelA;
                        entryModel.HasSize = true;
                    }

                    headerModel.Entry.Add(entryModel);
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

        public static List<LightingModel> ReadLighting(Stream stream, HeaderModel header)
        {
            List<LightingModel> list = new();

            using (BinaryReader br = new(stream))
            {
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

        public static List<ActorModel> ReadActor(Stream stream, HeaderModel header)
        {
            List<ActorModel> list = new();

            using (BinaryReader br = new(stream))
            {
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

        public static List<ItemModel> ReadItem(Stream stream, HeaderModel header)
        {
            List<ItemModel> list = new();

            using (BinaryReader br = new(stream))
            {
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

        public static List<EffectModel> ReadEffect(Stream stream, HeaderModel header)
        {
            List<EffectModel> list = new();

            using (BinaryReader br = new(stream))
            {
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

        public static List<BoundaryModel> ReadBoundary(Stream stream, HeaderModel header)
        {
            List<BoundaryModel> list = new();

            using (BinaryReader br = new(stream))
            {
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

        public static List<AOTModel> ReadAOT(Stream stream, HeaderModel header)
        {
            List<AOTModel> list = new();

            using (BinaryReader br = new(stream))
            {
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

        public static List<TriggerModel> ReadTrigger(Stream stream, HeaderModel header)
        {
            List<TriggerModel> list = new();

            using (BinaryReader br = new(stream))
            {
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

        public static List<EventModel> ReadEvent(Stream stream, HeaderModel header)
        {
            List<EventModel> list = new();

            using (BinaryReader br = new(stream))
            {
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