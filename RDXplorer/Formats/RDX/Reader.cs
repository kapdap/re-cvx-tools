using RDXplorer.Extensions;
using RDXplorer.Models.RDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RDXplorer.Formats.RDX
{
    public static class Reader
    {
        public static HeaderModel ReadHeader(FileInfo file)
        {
            HeaderModel header = new();

            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            header.Version.SetValue(fs.Position, br.ReadBytes(4));

            if (header.Version.Value == 0x41200000 ||
                header.Version.Value == 0x40051EB8)
            {
                fs.Seek(16, SeekOrigin.Begin);
                header.Tables.SetValue(fs.Position, br.ReadBytes(4));
                header.Model.SetValue(fs.Position, br.ReadBytes(4));
                header.Motion.SetValue(fs.Position, br.ReadBytes(4));
                header.Script.SetValue(fs.Position, br.ReadBytes(4));
                header.Texture.SetValue(fs.Position, br.ReadBytes(4));

                fs.Seek(96, SeekOrigin.Begin);
                header.Author.SetValue(fs.Position, br.ReadBytes(32));

                fs.Seek(header.Tables.Value, SeekOrigin.Begin);
                header.Camera.SetValue(fs.Position, br.ReadBytes(4));
                header.Lighting.SetValue(fs.Position, br.ReadBytes(4));
                header.Actor.SetValue(fs.Position, br.ReadBytes(4));
                header.Object.SetValue(fs.Position, br.ReadBytes(4));
                header.Item.SetValue(fs.Position, br.ReadBytes(4));
                header.Effect.SetValue(fs.Position, br.ReadBytes(4));
                header.Boundary.SetValue(fs.Position, br.ReadBytes(4));
                header.AOT.SetValue(fs.Position, br.ReadBytes(4));
                header.Trigger.SetValue(fs.Position, br.ReadBytes(4));
                header.Player.SetValue(fs.Position, br.ReadBytes(4));
                header.Event.SetValue(fs.Position, br.ReadBytes(4));
                header.Unknown1.SetValue(fs.Position, br.ReadBytes(4));
                header.Unknown2.SetValue(fs.Position, br.ReadBytes(4));
                header.Action.SetValue(fs.Position, br.ReadBytes(4));
                header.Text.SetValue(fs.Position, br.ReadBytes(4));
                header.Sysmes.SetValue(fs.Position, br.ReadBytes(4));

                fs.Seek(256, SeekOrigin.Begin);
                header.Camera.Count.SetValue(fs.Position, br.ReadBytes(4));
                header.Lighting.Count.SetValue(fs.Position, br.ReadBytes(4));
                header.Actor.Count.SetValue(fs.Position, br.ReadBytes(4));
                header.Object.Count.SetValue(fs.Position, br.ReadBytes(4));
                header.Item.Count.SetValue(fs.Position, br.ReadBytes(4));
                header.Effect.Count.SetValue(fs.Position, br.ReadBytes(4));
                header.Boundary.Count.SetValue(fs.Position, br.ReadBytes(4));
                header.AOT.Count.SetValue(fs.Position, br.ReadBytes(4));
                header.Trigger.Count.SetValue(fs.Position, br.ReadBytes(4));
                header.Player.Count.SetValue(fs.Position, br.ReadBytes(4));
                header.Event.Count.SetValue(fs.Position, br.ReadBytes(4));
                header.Unknown1.Count.SetValue(fs.Position, br.ReadBytes(4));

                header.Text.Count.SetValue(fs.Position, br.ReadBytes(4));
                header.Action.Count.SetValue(fs.Position, br.ReadBytes(4));

                fs.Seek(header.Texture.Value, SeekOrigin.Begin);
                header.Texture.Count.SetValue(fs.Position, br.ReadBytes(4));
            }

            return header;
        }

        public static List<ModelTableModel> ReadModels(FileInfo file, HeaderModel header)
        {
            List<ModelTableModel> list = new();

            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            fs.Seek(header.Model.Value, SeekOrigin.Begin);

            for (int i = 0; i < 256; i++)
            {
                ModelTableModel tableModel = new();

                tableModel.Position = (nint)fs.Position;
                tableModel.Fields.Pointer.SetValue(fs.Position, br.ReadBytes(4));

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

                fs.Seek(tableModel.Fields.Pointer.Value, SeekOrigin.Begin);

                while (fs.Position < nextOffset)
                {
                    ModelBlockModel blockModel = new();

                    blockModel.Table = tableModel;
                    blockModel.Position = (nint)fs.Position;

                    tableModel.Blocks.Add(blockModel);

                    DataEntryModel<uint> dataModelA = new();
                    dataModelA.SetValue(fs.Position, br.ReadBytes(4));

                    DataEntryModel<uint> dataModelB = new();
                    dataModelB.SetValue(fs.Position, br.ReadBytes(4));

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
                        fs.Seek(blockModel.Fields.Size.Value, SeekOrigin.Current);

                        while (fs.Position < nextOffset)
                        {
                            DataEntryModel<uint> dataModelV = new();
                            dataModelV.SetValue(fs.Position, br.ReadBytes(4));

                            if (dataModelV.Text == "SKIN" ||
                                dataModelV.Text == "MASK" ||
                                dataModelV.Text.StartsWith("MDL"))
                            {
                                fs.Seek(-8, SeekOrigin.Current);
                                break;
                            }
                        }
                    }
                    else
                        break;
                }
            }

            return list;
        }

        public static List<MotionTableModel> ReadMotions(FileInfo file, HeaderModel header)
        {
            List<MotionTableModel> list = new();

            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            fs.Seek(header.Motion.Value, SeekOrigin.Begin);

            uint firstPosition = 0;

            while (fs.Position < (firstPosition > 0 ? firstPosition : header.Script.Value))
            {
                MotionTableModel tableModel = new();

                tableModel.Position = (nint)fs.Position;
                tableModel.Fields.Pointer.SetValue(fs.Position, br.ReadBytes(4));

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

                fs.Seek(tableModel.Fields.Pointer.Value, SeekOrigin.Begin);

                uint nextOffset = i < list.Count - 1 ? list[i + 1].Fields.Pointer.Value : header.Script.Value;

                while (fs.Position < nextOffset)
                {
                    MotionBlockModel blockModel = new();

                    blockModel.Table = tableModel;
                    blockModel.Position = (nint)fs.Position;

                    tableModel.Blocks.Add(blockModel);

                    blockModel.Fields.Size.SetValue(fs.Position, br.ReadBytes(4));
                    blockModel.Fields.Type.SetValue(fs.Position, br.ReadBytes(4));

                    fs.Seek(-4, SeekOrigin.Current);

                    blockModel.Fields.Data.SetValue(fs.Position, br.ReadBytes((int)blockModel.Fields.Size.Value));

                    while (fs.Position < nextOffset)
                    {
                        DataEntryModel<uint> dataModel = new();
                        dataModel.SetValue(fs.Position, br.ReadBytes(4));

                        if (dataModel.Value != 0 && dataModel.Value != 0xFFFFFFFF)
                        {
                            fs.Seek(-4, SeekOrigin.Current);
                            break;
                        }
                    }
                }
            }

            return list;
        }

        public static List<ScriptModel> ReadScripts(FileInfo file, HeaderModel header)
        {
            List<ScriptModel> list = new();

            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            fs.Seek(header.Script.Value, SeekOrigin.Begin);

            while (fs.Position < (list.Count > 0 ? list[0].Pointer.Value : fs.Position + 1))
            {
                ScriptModel model = new();

                model.Position = (nint)fs.Position;

                model.Fields.Offset.SetValue(fs.Position, br.ReadBytes(4));
                model.Pointer.SetValue(model.Position, BitConverter.GetBytes(header.Script.Value + model.Fields.Offset.Value));

                list.Add(model);
            }

            for (int i = 0; i < list.Count; i++)
            {
                uint nextOffset = i < list.Count - 1 ? list[i + 1].Pointer.Value : header.Texture.Value;

                ScriptModel model = list[i];

                fs.Seek(model.Pointer.Value, SeekOrigin.Begin);

                model.Size = nextOffset - model.Pointer.Value;
                model.Fields.Data.SetValue(fs.Position, br.ReadBytes((int)model.Size));
            }

            return list;
        }

        public static List<TextureTableModel> ReadTextures(FileInfo file, HeaderModel header)
        {
            List<TextureTableModel> list = new();

            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            fs.Seek(header.Texture.Value + 4, SeekOrigin.Begin);

            for (int i = 0; i < header.Texture.Count.Value; i++)
            {
                TextureTableModel model = new();

                model.Position = (nint)fs.Position;
                model.Fields.Pointer.SetValue(fs.Position, br.ReadBytes(4));

                if (model.Fields.Pointer.Value == 0)
                    break;

                list.Add(model);
            }

            for (int i = 0; i < list.Count; i++)
            {
                uint nextOffset = i < list.Count - 1 ? list[i + 1].Fields.Pointer.Value : (uint)file.Length;

                TextureTableModel tableModel = list[i];
                tableModel.Size = nextOffset - tableModel.Fields.Pointer.Value;

                fs.Seek(tableModel.Fields.Pointer.Value, SeekOrigin.Begin);

                while (fs.Position < nextOffset)
                {
                    TextureBlockModel blockModel = new();

                    blockModel.Position = (nint)fs.Position;
                    blockModel.Table = tableModel;

                    blockModel.Fields.Type.SetValue(fs.Position, br.ReadBytes(4));

                    if (blockModel.Fields.Type.Value == 0xFFFFFFFF)
                        break;

                    blockModel.Fields.Size.SetValue(fs.Position, br.ReadBytes(4));

                    tableModel.Blocks.Add(blockModel);

                    fs.Seek(24, SeekOrigin.Current);
                    fs.Seek(blockModel.Fields.Size.Value, SeekOrigin.Current);
                }
            }

            return list;
        }

        public static List<CameraHeaderModel> ReadCamera(FileInfo file, HeaderModel header)
        {
            List<CameraHeaderModel> list = new();

            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            fs.Seek(header.Camera.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Camera.Count.Value; i++)
            {
                CameraHeaderModel camera = new();

                camera.Position = (nint)fs.Position;

                camera.Fields.Flag1.SetValue(fs.Position, br.ReadBytes(1));
                camera.Fields.Flag2.SetValue(fs.Position, br.ReadBytes(1));
                camera.Fields.Flag3.SetValue(fs.Position, br.ReadBytes(1));
                camera.Fields.Flag4.SetValue(fs.Position, br.ReadBytes(1));
                camera.Fields.Pointer.SetValue(fs.Position, br.ReadBytes(4));

                for (int j = 0; j < 3; j++)
                {
                    CameraBlockModel model = new();

                    model.Position = (nint)fs.Position;
                    model.Header = camera;

                    model.Fields.Unknown1.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown2.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown3.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown4.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown5.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown6.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown7.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.X.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Y.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Z.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown11.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown12.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown13.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown14.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown15.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown16.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown17.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown18.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown19.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.XRotation.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.YRotation.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.ZRotation.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown23.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown24.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown25.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown26.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown27.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Perspective.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown29.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown30.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown31.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown32.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown33.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown34.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown35.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown36.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown37.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown38.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown39.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown40.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown41.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown42.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown43.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown44.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown45.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown46.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown47.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown48.SetValue(fs.Position, br.ReadBytes(4));
                    model.Fields.Unknown49.SetValue(fs.Position, br.ReadBytes(4));

                    camera.Blocks.Add(model);

                    fs.Seek(28, SeekOrigin.Current);
                }

                list.Add(camera);
            }

            return list;
        }

        public static List<LightingModel> ReadLighting(FileInfo file, HeaderModel header)
        {
            List<LightingModel> list = new();

            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            fs.Seek(header.Lighting.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Lighting.Count.Value; i++)
            {
                LightingModel model = new();

                model.Position = (nint)fs.Position;

                model.Fields.Unknown1.SetValue(fs.Position, br.ReadBytes(2));
                model.Fields.Unknown2.SetValue(fs.Position, br.ReadBytes(2));
                model.Fields.Unknown3.SetValue(fs.Position, br.ReadBytes(4));

                model.Fields.Unknown4.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown5.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown6.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown7.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown8.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown9.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown10.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown11.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown12.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown13.SetValue(fs.Position, br.ReadBytes(4));

                model.Fields.Data.SetValue(fs.Position, br.ReadBytes(0xB0));

                list.Add(model);
            }

            return list;
        }

        public static List<ActorModel> ReadActor(FileInfo file, HeaderModel header)
        {
            List<ActorModel> list = new();

            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            fs.Seek(header.Actor.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Actor.Count.Value; i++)
            {
                ActorModel model = new();

                model.Position = (nint)fs.Position;

                model.Fields.Unknown0.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Type.SetValue(fs.Position, br.ReadBytes(2));
                model.Fields.Unknown1.SetValue(fs.Position, br.ReadBytes(2));
                model.Fields.Unknown2.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown3.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Slot.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown4.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.X.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Y.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Z.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown5.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Rotation.SetValue(fs.Position, br.ReadBytes(2));
                model.Fields.Unknown6.SetValue(fs.Position, br.ReadBytes(2));
                model.Fields.Unknown7.SetValue(fs.Position, br.ReadBytes(4));

                list.Add(model);
            }

            return list;
        }

        public static List<ObjectModel> ReadObject(FileInfo file, HeaderModel header)
        {
            List<ObjectModel> list = new();

            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            fs.Seek(header.Object.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Object.Count.Value; i++)
            {
                ObjectModel model = new();

                model.Position = (nint)fs.Position;

                model.Fields.Visible.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown1.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown2.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown3.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Type.SetValue(fs.Position, br.ReadBytes(2));
                model.Fields.Unknown4.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown5.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown6.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown7.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown8.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown9.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.X.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Y.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Z.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.XRot.SetValue(fs.Position, br.ReadBytes(2));
                model.Fields.YRot.SetValue(fs.Position, br.ReadBytes(2));
                model.Fields.ZRot.SetValue(fs.Position, br.ReadBytes(2));
                model.Fields.Unknown10.SetValue(fs.Position, br.ReadBytes(2));
                model.Fields.Unknown11.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown12.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown13.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown14.SetValue(fs.Position, br.ReadBytes(1));

                list.Add(model);
            }

            return list;
        }

        public static List<ItemModel> ReadItem(FileInfo file, HeaderModel header)
        {
            List<ItemModel> list = new();

            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            fs.Seek(header.Item.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Item.Count.Value; i++)
            {
                ItemModel model = new();

                model.Position = (nint)fs.Position;

                model.Fields.Unknown1.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown2.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown3.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown4.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Type.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown5.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.X.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Y.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Z.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.XRot.SetValue(fs.Position, br.ReadBytes(2));
                model.Fields.YRot.SetValue(fs.Position, br.ReadBytes(2));
                model.Fields.ZRot.SetValue(fs.Position, br.ReadBytes(2));
                model.Fields.Unknown6.SetValue(fs.Position, br.ReadBytes(2));
                model.Fields.Unknown7.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown8.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown9.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown10.SetValue(fs.Position, br.ReadBytes(1));

                list.Add(model);
            }

            return list;
        }

        public static List<EffectModel> ReadEffect(FileInfo file, HeaderModel header)
        {
            List<EffectModel> list = new();

            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            fs.Seek(header.Effect.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Effect.Count.Value; i++)
            {
                EffectModel model = new();

                model.Position = (nint)fs.Position;

                model.Fields.Unknown1.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown2.SetValue(fs.Position, br.ReadBytes(2));
                model.Fields.Unknown3.SetValue(fs.Position, br.ReadBytes(2));
                model.Fields.Unknown4.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.X.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Y.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Z.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Width.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Height.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Length.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown11.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown12.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown13.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown14.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown15.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown16.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown17.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown18.SetValue(fs.Position, br.ReadBytes(4));

                list.Add(model);
            }

            return list;
        }

        public static List<BoundaryModel> ReadBoundary(FileInfo file, HeaderModel header)
        {
            List<BoundaryModel> list = new();

            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            fs.Seek(header.Boundary.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Boundary.Count.Value; i++)
            {
                BoundaryModel model = new();

                model.Position = (nint)fs.Position;

                model.Fields.Unknown1.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown2.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown3.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown4.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown5.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.X.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Y.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Z.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Width.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Height.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Length.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown12.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown13.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown14.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown15.SetValue(fs.Position, br.ReadBytes(1));

                list.Add(model);
            }

            return list;
        }

        public static List<AOTModel> ReadAOT(FileInfo file, HeaderModel header)
        {
            List<AOTModel> list = new();

            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            fs.Seek(header.AOT.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.AOT.Count.Value; i++)
            {
                AOTModel model = new();

                model.Position = (nint)fs.Position;

                model.Fields.Unknown1.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Type.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown3.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown4.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown5.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.X.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Y.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Z.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Width.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Height.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Length.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown12.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown13.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown14.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown15.SetValue(fs.Position, br.ReadBytes(1));

                list.Add(model);
            }

            return list;
        }

        public static List<TriggerModel> ReadTrigger(FileInfo file, HeaderModel header)
        {
            List<TriggerModel> list = new();

            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            fs.Seek(header.Trigger.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Trigger.Count.Value; i++)
            {
                TriggerModel model = new();

                model.Position = (nint)fs.Position;

                model.Fields.Unknown1.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown2.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown3.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown4.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown5.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.X.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Y.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Z.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Width.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Height.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Length.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown12.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown13.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown14.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown15.SetValue(fs.Position, br.ReadBytes(1));

                list.Add(model);
            }

            return list;
        }

        public static List<PlayerModel> ReadPlayer(FileInfo file, HeaderModel header)
        {
            List<PlayerModel> list = new();

            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            fs.Seek(header.Player.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Player.Count.Value; i++)
            {
                PlayerModel model = new();

                model.Position = (nint)fs.Position;

                model.Fields.X.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Y.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Z.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Rotation.SetValue(fs.Position, br.ReadBytes(4));

                list.Add(model);
            }

            return list;
        }

        public static List<EventModel> ReadEvent(FileInfo file, HeaderModel header)
        {
            List<EventModel> list = new();

            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            fs.Seek(header.Event.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Event.Count.Value; i++)
            {
                EventModel model = new();

                model.Position = (nint)fs.Position;

                model.Fields.Unknown1.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown2.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown3.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown4.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown5.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.X.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Y.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Z.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Width.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Height.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Length.SetValue(fs.Position, br.ReadBytes(4));
                model.Fields.Unknown12.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown13.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown14.SetValue(fs.Position, br.ReadBytes(1));
                model.Fields.Unknown15.SetValue(fs.Position, br.ReadBytes(1));

                list.Add(model);
            }

            return list;
        }

        public static List<TextModel> ReadTexts(FileInfo file, HeaderModel header)
        {
            List<TextModel> list = new();

            if (header.Text.Value == 0)
                return list;

            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            fs.Seek(header.Text.Value, SeekOrigin.Begin);

            int count = br.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                TextModel model = new();

                model.Position = (nint)fs.Position;

                model.Fields.Offset.SetValue(fs.Position, br.ReadBytes(4));
                model.Pointer.SetValue(model.Position, BitConverter.GetBytes(header.Text.Value + model.Fields.Offset.Value));

                list.Add(model);
            }

            uint nextSection = header.Sysmes.Value != 0 ? header.Sysmes.Value : header.Model.Value;

            for (int i = 0; i < list.Count; i++)
            {
                uint nextOffset = i < list.Count - 1 ? list[i + 1].Pointer.Value : nextSection;

                TextModel model = list[i];

                fs.Seek(model.Pointer.Value, SeekOrigin.Begin);

                model.Size = nextOffset - model.Pointer.Value;

                StringBuilder message = new();

                while (fs.Position < nextOffset)
                {
                    ushort text = br.ReadUInt16();

                    if (text == 0xFF00)
                        message.Append(Environment.NewLine);
                    else if (text == 0xFF02)
                        message.Append($"[WAIT:{br.ReadInt16()}]");
                    else if (text == 0xFF03)
                        message.Append($"[ITEM:{br.ReadInt16()}]");
                    else if (text == 0xFFFF)
                        break;
                    else
                        message.Append(DecodeText(text));
                }

                model.Message = message.ToString();
            }

            return list;
        }

        private static Dictionary<string, string> _characterMap = new();
        private static Dictionary<string, string> CharacterMap
        {
            get
            {
                if (_characterMap.Count == 0)
                {
                    // TODO: Create Tables for JPN and EUR releases
                    FileInfo file = new(Path.Combine(AppContext.BaseDirectory, "Data\\ASCII\\BIOCV_USA.tbl"));

                    if (file.Exists)
                    {
                        string[] lines = File.ReadAllLines(file.FullName);

                        foreach (string line in lines)
                        {
                            string[] parts = line.Split("=");

                            if (parts.Length >= 2)
                                _characterMap.Add(parts[0], parts[1]);
                        }
                    }
                }

                return _characterMap;
            }
        }

        private static string DecodeText(ushort value)
        {
            string key = Utilities.SwapBytes(value).ToString("X4");

            return CharacterMap.ContainsKey(key) ? CharacterMap[key] : $"[0x{key}]";
        }
    }
}
