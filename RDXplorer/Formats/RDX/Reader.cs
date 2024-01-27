using RDXplorer.Models.RDX;
using System;
using System.Collections.Generic;
using System.IO;

namespace RDXplorer.Formats.RDX
{
    public static class Reader
    {
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

                    tableModel.Position = (nint)stream.Position;
                    tableModel.Fields.Pointer.SetValue(stream.Position, br.ReadBytes(4));

                    if (tableModel.Fields.Pointer.Value == 0 ||
                        tableModel.Fields.Pointer.Text == "SKIN" ||
                        tableModel.Fields.Pointer.Text.StartsWith("MDL"))
                        break;

                    list.Add(tableModel);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    uint nextOffset = i < list.Count - 1 ? list[i + 1].Fields.Pointer.Value : header.Motion.Value;

                    ModelTableModel tableModel = list[i];
                    tableModel.Size = nextOffset - tableModel.Fields.Pointer.Value;

                    stream.Seek(tableModel.Fields.Pointer.Value, SeekOrigin.Begin);

                    while (stream.Position < nextOffset)
                    {
                        ModelBlockModel blockModel = new();

                        blockModel.Table = tableModel;
                        blockModel.Position = (nint)stream.Position;

                        tableModel.Blocks.Add(blockModel);

                        DataEntryModel<uint> dataModelA = new();
                        dataModelA.SetValue(stream.Position, br.ReadBytes(4));

                        DataEntryModel<uint> dataModelB = new();
                        dataModelB.SetValue(stream.Position, br.ReadBytes(4));

                        if (dataModelA.Text == "SKIN" ||
                            dataModelA.Text.StartsWith("MDL"))
                        {
                            blockModel.Fields.Type = dataModelA;
                        }
                        else
                        {
                            blockModel.Fields.Type = dataModelB;
                            blockModel.Fields.Size = dataModelA;
                            blockModel.HasSize = true;
                        }

                        if (blockModel.HasSize)
                        {
                            stream.Seek(blockModel.Fields.Size.Value, SeekOrigin.Current);

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

                    tableModel.Position = (nint)stream.Position;
                    tableModel.Fields.Pointer.SetValue(stream.Position, br.ReadBytes(4));

                    if (tableModel.Fields.Pointer.Value > header.Script.Value)
                        break;

                    if (firstPosition == 0)
                        firstPosition = tableModel.Fields.Pointer.Value;

                    if (tableModel.Fields.Pointer.Value != 0)
                        list.Add(tableModel);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    MotionTableModel tableModel = list[i];

                    if (tableModel.Fields.Pointer.Value == 0)
                        continue;

                    stream.Seek(tableModel.Fields.Pointer.Value, SeekOrigin.Begin);

                    uint nextOffset = i < list.Count - 1 ? list[i + 1].Fields.Pointer.Value : header.Script.Value;

                    while (stream.Position < nextOffset)
                    {
                        MotionBlockModel blockModel = new();

                        blockModel.Table = tableModel;
                        blockModel.Position = (nint)stream.Position;

                        tableModel.Blocks.Add(blockModel);

                        blockModel.Fields.Size.SetValue(stream.Position, br.ReadBytes(4));
                        blockModel.Fields.Type.SetValue(stream.Position, br.ReadBytes(4));

                        stream.Seek(-4, SeekOrigin.Current);

                        blockModel.Fields.Data.SetValue(stream.Position, br.ReadBytes((int)blockModel.Fields.Size.Value));

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

                    model.Position = (nint)stream.Position;

                    model.Fields.Offset.SetValue(stream.Position, br.ReadBytes(4));
                    model.Pointer.SetValue(model.Position, BitConverter.GetBytes(header.Script.Value + model.Fields.Offset.Value));

                    list.Add(model);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    uint nextOffset = i < list.Count - 1 ? list[i + 1].Pointer.Value : header.Texture.Value;

                    ScriptModel model = list[i];

                    stream.Seek(model.Pointer.Value, SeekOrigin.Begin);

                    model.Size = nextOffset - model.Pointer.Value;
                    model.Fields.Data.SetValue(stream.Position, br.ReadBytes((int)model.Size));
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

                    model.Position = (nint)stream.Position;
                    model.Fields.Pointer.SetValue(stream.Position, br.ReadBytes(4));

                    if (model.Fields.Pointer.Value == 0)
                        break;

                    list.Add(model);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    uint nextOffset = i < list.Count - 1 ? list[i + 1].Fields.Pointer.Value : (uint)file.Length;

                    TextureTableModel tableModel = list[i];
                    tableModel.Size = nextOffset - tableModel.Fields.Pointer.Value;

                    stream.Seek(tableModel.Fields.Pointer.Value, SeekOrigin.Begin);

                    while (stream.Position < nextOffset)
                    {
                        TextureBlockModel blockModel = new();

                        blockModel.Position = (nint)stream.Position;
                        blockModel.Table = tableModel;

                        blockModel.Fields.Type.SetValue(stream.Position, br.ReadBytes(4));

                        if (blockModel.Fields.Type.Value == 0xFFFFFFFF)
                            break;

                        blockModel.Fields.Size.SetValue(stream.Position, br.ReadBytes(4));

                        tableModel.Blocks.Add(blockModel);

                        stream.Seek(24, SeekOrigin.Current);
                        stream.Seek(blockModel.Fields.Size.Value, SeekOrigin.Current);
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

                    model.Position = (nint)stream.Position;

                    model.Fields.Unknown1.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown2.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown3.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown4.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Pointer.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown6.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown7.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown8.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown9.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown10.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown11.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown12.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown16.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown17.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown18.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown19.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown20.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown21.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown22.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown23.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown24.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.XRotation.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.YRotation.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.ZRotation.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown28.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown29.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown30.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown31.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown32.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Perspective.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown34.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown35.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown36.SetValue(stream.Position, br.ReadBytes(0x228));

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

                    model.Position = (nint)stream.Position;

                    model.Fields.Unknown1.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Unknown2.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Unknown3.SetValue(stream.Position, br.ReadBytes(4));

                    model.Fields.Unknown4.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown5.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown6.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown7.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown8.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown9.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown10.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown11.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown12.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown13.SetValue(stream.Position, br.ReadBytes(4));

                    model.Fields.Data.SetValue(stream.Position, br.ReadBytes(0xB0));

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

                    model.Position = (nint)stream.Position;

                    model.Fields.Unknown0.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Type.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Unknown1.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Unknown2.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown3.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Slot.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown4.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown5.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Rotation.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Unknown6.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Unknown7.SetValue(stream.Position, br.ReadBytes(4));

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

                    model.Position = (nint)stream.Position;

                    model.Fields.Visible.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown1.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown2.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown3.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Type.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Unknown4.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown5.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown6.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown7.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown8.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown9.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.XRot.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.YRot.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.ZRot.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Unknown10.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Unknown11.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown12.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown13.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown14.SetValue(stream.Position, br.ReadBytes(1));

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

                    model.Position = (nint)stream.Position;

                    model.Fields.Unknown1.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown2.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown3.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown4.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Type.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown5.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.XRot.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.YRot.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.ZRot.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Unknown6.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Unknown7.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown8.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown9.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown10.SetValue(stream.Position, br.ReadBytes(1));

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

                    model.Position = (nint)stream.Position;

                    model.Fields.Unknown1.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown2.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Unknown3.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Unknown4.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Width.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Height.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Length.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown11.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown12.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown13.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown14.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown15.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown16.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown17.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown18.SetValue(stream.Position, br.ReadBytes(4));

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

                    model.Position = (nint)stream.Position;

                    model.Fields.Unknown1.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown2.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown3.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown4.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown5.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Width.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Height.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Length.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown12.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown13.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown14.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown15.SetValue(stream.Position, br.ReadBytes(1));

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

                    model.Position = (nint)stream.Position;

                    model.Fields.Unknown1.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Type.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown3.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown4.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown5.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Width.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Height.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Length.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown12.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown13.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown14.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown15.SetValue(stream.Position, br.ReadBytes(1));

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

                    model.Position = (nint)stream.Position;

                    model.Fields.Unknown1.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown2.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown3.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown4.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown5.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Width.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Height.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Length.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown12.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown13.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown14.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown15.SetValue(stream.Position, br.ReadBytes(1));

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

                    model.Position = (nint)stream.Position;

                    model.Fields.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Rotation.SetValue(stream.Position, br.ReadBytes(4));

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

                    model.Position = (nint)stream.Position;

                    model.Fields.Unknown1.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown2.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown3.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown4.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown5.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.X.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Y.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Z.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Width.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Height.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Length.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Unknown12.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown13.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown14.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Unknown15.SetValue(stream.Position, br.ReadBytes(1));

                    list.Add(model);
                }
            }

            return list;
        }
    }
}
