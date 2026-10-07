using PSO.PRS;
using RDXplorer.Extensions;
using RDXplorer.Models.RDX;
using RDXplorer.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RDXplorer.Formats.RDX
{
    public static class Reader
    {
        public const int RDX_MAGIC_1 = 0x41200000;
        public const int RDX_MAGIC_2 = 0x40051EB8;
        public const int RDX_MAGIC_3 = 0x40028F5C;
        public const int RDX_MAGIC_PRS_1 = 0x200000DF;
        public const int RDX_MAGIC_PRS_2 = 0x051EB8DF;
        public const int RDX_MAGIC_PRS_3 = 0x028F5CDF;

        public static DocumentModel LoadFile(FileInfo file) =>
            LoadFile(file, Program.TempPath);

        public static DocumentModel LoadFile(FileInfo file, DirectoryInfo output)
        {
            FileInfo prs = new(file.FullName);

            if (IsPRS(prs))
                file = ExtractPRS(prs, output);
            else
                prs = null;

            return IsValid(file) ? new(file, prs) : null;
        }

        public static bool IsValid(FileInfo file)
        {
            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            int magic = br.ReadInt32();

            return magic == RDX_MAGIC_1 || magic == RDX_MAGIC_2 || magic == RDX_MAGIC_3;
        }

        public static bool IsPRS(FileInfo file)
        {
            using FileStream fs = file.OpenReadShared();
            using BinaryReader br = new(fs);

            int magic = br.ReadInt32();

            return magic == RDX_MAGIC_PRS_1 || magic == RDX_MAGIC_PRS_2 || magic == RDX_MAGIC_PRS_3;
        }

        public static FileInfo ExtractPRS(FileInfo prs, DirectoryInfo output)
        {
            using FileStream fs = prs.OpenReadShared();
            using BinaryReader br = new(fs);

            FileInfo file = new(Path.Combine(output.FullName, Utilities.GetFileMD5(fs)));

            if (!file.Directory.Exists)
                file.Directory.Create();

            if (!file.Exists)
                File.WriteAllBytes(file.FullName, PRS.Decompress(br.ReadBytes((int)prs.Length)));

            file.Refresh();

            return file;
        }

        public static HeaderModel ReadHeader(FileInfo file)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadHeader(stream);
        }

        public static HeaderModel ReadHeader(Stream stream)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            HeaderModel header = new();

            stream.Position = 0;

            header.Version.SetValue(stream.Position, br.ReadBytes(4));

            if (header.Version.Value == 0x41200000 ||
                header.Version.Value == 0x40051EB8 ||
                header.Version.Value == 0x40028F5C)
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
                header.Enemy.SetValue(stream.Position, br.ReadBytes(4));
                header.Object.SetValue(stream.Position, br.ReadBytes(4));
                header.Item.SetValue(stream.Position, br.ReadBytes(4));
                header.Effect.SetValue(stream.Position, br.ReadBytes(4));
                header.Boundary.SetValue(stream.Position, br.ReadBytes(4));
                header.AOT.SetValue(stream.Position, br.ReadBytes(4));
                header.Trigger.SetValue(stream.Position, br.ReadBytes(4));
                header.Player.SetValue(stream.Position, br.ReadBytes(4));
                header.Route.SetValue(stream.Position, br.ReadBytes(4));
                header.RouteTable.SetValue(stream.Position, br.ReadBytes(4));
                header.EventScript.SetValue(stream.Position, br.ReadBytes(4));
                header.EventCamera.SetValue(stream.Position, br.ReadBytes(4));
                header.Text.SetValue(stream.Position, br.ReadBytes(4));
                header.EventLight.SetValue(stream.Position, br.ReadBytes(4));

                stream.Seek(256, SeekOrigin.Begin);
                header.Camera.Count.SetValue(stream.Position, br.ReadBytes(4));
                header.Lighting.Count.SetValue(stream.Position, br.ReadBytes(4));
                header.Enemy.Count.SetValue(stream.Position, br.ReadBytes(4));
                header.Object.Count.SetValue(stream.Position, br.ReadBytes(4));
                header.Item.Count.SetValue(stream.Position, br.ReadBytes(4));
                header.Effect.Count.SetValue(stream.Position, br.ReadBytes(4));
                header.Boundary.Count.SetValue(stream.Position, br.ReadBytes(4));
                header.AOT.Count.SetValue(stream.Position, br.ReadBytes(4));
                header.Trigger.Count.SetValue(stream.Position, br.ReadBytes(4));
                header.Player.Count.SetValue(stream.Position, br.ReadBytes(4));
                header.Route.Count.SetValue(stream.Position, br.ReadBytes(4));
                header.EventCamera.Count.SetValue(stream.Position, br.ReadBytes(4));

                header.Text.Count.SetValue(stream.Position, br.ReadBytes(4));
                header.EventLight.Count.SetValue(stream.Position, br.ReadBytes(4));

                stream.Seek(header.Texture.Value, SeekOrigin.Begin);
                header.Texture.Count.SetValue(stream.Position, br.ReadBytes(4));
            }

            return header;
        }

        public static List<ModelTableModel> ReadModels(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadModels(stream, header);
        }

        public static List<ModelTableModel> ReadModels(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<ModelTableModel> list = new();

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

                if (tableModel.Fields.Pointer.Value <= header.Model.Value ||
                    tableModel.Fields.Pointer.Value >= header.Motion.Value ||
                    (list.Count > 0 && tableModel.Fields.Pointer.Value <= list[^1].Fields.Pointer.Value))
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

            return list;
        }

        public static List<MotionTableModel> ReadMotions(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadMotions(stream, header);
        }

        public static List<MotionTableModel> ReadMotions(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<MotionTableModel> list = new();

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

                    uint size = blockModel.Fields.Size.Value;

                    if (size == 0 ||
                        (ulong)stream.Position - 4 + size > nextOffset ||
                        (ulong)stream.Position - 4 + size > (ulong)stream.Length)
                    {
                        tableModel.Blocks.Remove(blockModel);
                        break;
                    }

                    stream.Seek(-4, SeekOrigin.Current);

                    blockModel.Fields.Data.SetValue(stream.Position, br.ReadBytes((int)size));

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

            return list;
        }

        public static List<ScriptModel> ReadScripts(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadScripts(stream, header);
        }

        public static List<ScriptModel> ReadScripts(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<ScriptModel> list = new();

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

            return list;
        }

        public static List<TextureTableModel> ReadTextures(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadTextures(stream, header);
        }

        public static List<TextureTableModel> ReadTextures(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<TextureTableModel> list = new();

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
                uint nextOffset = i < list.Count - 1 ? list[i + 1].Fields.Pointer.Value : (uint)stream.Length;

                TextureTableModel tableModel = list[i];
                tableModel.Size = nextOffset - tableModel.Fields.Pointer.Value;

                stream.Seek(tableModel.Fields.Pointer.Value, SeekOrigin.Begin);

                while (stream.Position < nextOffset)
                {
                    TextureBlockModel blockModel = new();

                    blockModel.Position = (nint)stream.Position;
                    blockModel.Table = tableModel;

                    blockModel.Fields.Type.SetValue(stream.Position, br.ReadBytes(4));

                    // 0xFFFFFFFF is the end of the block
                    if (blockModel.Fields.Type.Value == 0xFFFFFFFF)
                        break;

                    blockModel.Fields.Size.SetValue(stream.Position, br.ReadBytes(4));

                    uint size = blockModel.Fields.Size.Value;

                    if (blockModel.Fields.Type.Value == 0 && size == 0)
                        break;

                    if ((ulong)stream.Position + 24 + size > nextOffset ||
                        (ulong)stream.Position + 24 + size > (ulong)stream.Length)
                        break;

                    blockModel.Fields.Head.SetValue(stream.Position, br.ReadBytes(24));

                    if (size > 0)
                        blockModel.Fields.Data.SetValue(stream.Position, br.ReadBytes((int)size));

                    tableModel.Blocks.Add(blockModel);
                }
            }

            return list;
        }

        public static List<CameraHeaderModel> ReadCamera(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadCamera(stream, header);
        }

        public static List<CameraHeaderModel> ReadCamera(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<CameraHeaderModel> list = new();

            stream.Seek(header.Camera.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Camera.Count.Value; i++)
            {
                CameraHeaderModel camera = new();

                camera.Position = (nint)stream.Position;

                camera.Fields.Flg.SetValue(stream.Position, br.ReadBytes(1));
                camera.Fields.Type.SetValue(stream.Position, br.ReadBytes(1));
                camera.Fields.FlrNo.SetValue(stream.Position, br.ReadBytes(1));
                camera.Fields.CTabN.SetValue(stream.Position, br.ReadBytes(1));
                camera.Fields.Cuttp.SetValue(stream.Position, br.ReadBytes(4));
                camera.Fields.Cx.SetValue(stream.Position, br.ReadBytes(4));
                camera.Fields.Cy.SetValue(stream.Position, br.ReadBytes(4));
                camera.Fields.Cz.SetValue(stream.Position, br.ReadBytes(4));
                camera.Fields.Cw.SetValue(stream.Position, br.ReadBytes(4));
                camera.Fields.Ch.SetValue(stream.Position, br.ReadBytes(4));
                camera.Fields.Cd.SetValue(stream.Position, br.ReadBytes(4));

                for (int j = 0; j < 2; j++)
                {
                    CameraBlockModel model = new();

                    model.Position = (nint)stream.Position;
                    model.Header = camera;

                    model.Fields.Flg.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.LgtClip.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Spd.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Px.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Py.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Pz.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Ln.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.W.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.H.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.D.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Y0.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Y1.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Y2.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Y3.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.AmSpd.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Ax.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Ay.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Az.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Lax.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Lay.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Laz0.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Laz1.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Laz2.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Laz3.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.AaSpd.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.FilNo.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.FilRt.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Reserve.SetValue(stream.Position, br.ReadBytes(1));
                    model.Fields.Pers.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj0.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj1.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj2.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj3.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj4.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj5.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj6.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj7.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj8.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj9.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj10.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj11.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj12.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj13.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj14.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj15.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidLgt0.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidLgt1.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidLgt2.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidLgt3.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidLgt4.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidLgt5.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidLgt6.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidLgt7.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.FogCol.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.FogNr.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.FogFr.SetValue(stream.Position, br.ReadBytes(4));

                    camera.Blocks.Add(model);
                }

                camera.Fields.Exd.SetValue(stream.Position, br.ReadBytes(0x100));

                list.Add(camera);
            }

            return list;
        }

        public static List<LightingModel> ReadLighting(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadLighting(stream, header);
        }

        public static List<LightingModel> ReadLighting(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<LightingModel> list = new();

            stream.Seek(header.Lighting.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Lighting.Count.Value; i++)
            {
                LightingModel model = new();

                model.Position = (nint)stream.Position;

                model.Fields.Flg.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Type.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Aspd.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.LkFlg.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.LkNo.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.LkOno.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Lsrc.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Px.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Py.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Pz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Lx.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ly.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Lz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Vx.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Vy.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Vz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Spc.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Dif.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Amb.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.R.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.G.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.B.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Nr.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Fr.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Iang.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Oang.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ax.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ay.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Az.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Mode.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ct0.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ct1.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ct2.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ct3.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wpx.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wpy.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wpz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wvx.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wvy.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wvz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wspc.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wdif.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wamb.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wr.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wg.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wb.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wnr.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wfr.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wiang.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Woang.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wax.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Way.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Waz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Lkwkp.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Exp.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Light.SetValue(stream.Position, br.ReadBytes(4));

                list.Add(model);
            }

            return list;
        }

        public static List<EnemyModel> ReadEnemy(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadEnemy(stream, header);
        }

        public static List<EnemyModel> ReadEnemy(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<EnemyModel> list = new();

            stream.Seek(header.Enemy.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Enemy.Count.Value; i++)
            {
                EnemyModel model = new();

                model.Position = (nint)stream.Position;

                model.Fields.Flg.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Id.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Type.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.FlrNo.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.MdlVer.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.WrkNo.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Prm1.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Px.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Py.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Pz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ax.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Az.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Ay.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Aspd.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Hide0.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Hide1.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Hide2.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Hide3.SetValue(stream.Position, br.ReadBytes(1));

                list.Add(model);
            }

            return list;
        }

        public static List<ObjectModel> ReadObject(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadObject(stream, header);
        }

        public static List<ObjectModel> ReadObject(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<ObjectModel> list = new();

            stream.Seek(header.Object.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Object.Count.Value; i++)
            {
                ObjectModel model = new();

                model.Position = (nint)stream.Position;

                model.Fields.Flg.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Id.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Type.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.FlrNo.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.MdlVer.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.WrkNo.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Prm1.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Px.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Py.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Pz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ax.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Az.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Ay.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Aspd.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Hide0.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Hide1.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Hide2.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Hide3.SetValue(stream.Position, br.ReadBytes(1));

                list.Add(model);
            }

            return list;
        }

        public static List<ItemModel> ReadItem(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadItem(stream, header);
        }

        public static List<ItemModel> ReadItem(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<ItemModel> list = new();

            stream.Seek(header.Item.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Item.Count.Value; i++)
            {
                ItemModel model = new();

                model.Position = (nint)stream.Position;

                model.Fields.Flg.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Id.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Type.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.FlrNo.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.MdlVer.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.WrkNo.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Prm1.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Px.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Py.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Pz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ax.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Az.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Ay.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Aspd.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Hide0.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Hide1.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Hide2.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Hide3.SetValue(stream.Position, br.ReadBytes(1));

                list.Add(model);
            }

            return list;
        }

        public static List<EffectModel> ReadEffect(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadEffect(stream, header);
        }

        public static List<EffectModel> ReadEffect(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<EffectModel> list = new();

            stream.Seek(header.Effect.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Effect.Count.Value; i++)
            {
                EffectModel model = new();

                model.Position = (nint)stream.Position;

                model.Fields.Flg.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Id.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Type.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.FlrNo.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.MdlVer.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Px.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Py.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Pz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Sx.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Sy.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Sz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ay.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.Ax.SetValue(stream.Position, br.ReadBytes(2));
                model.Fields.LkFlg.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.LkNo.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.LkOno.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Lx.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ly.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Lz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Param.SetValue(stream.Position, br.ReadBytes(4));

                list.Add(model);
            }

            return list;
        }

        public static List<BoundaryModel> ReadBoundary(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadBoundary(stream, header);
        }

        public static List<BoundaryModel> ReadBoundary(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<BoundaryModel> list = new();

            stream.Seek(header.Boundary.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Boundary.Count.Value; i++)
            {
                BoundaryModel model = new();

                model.Position = (nint)stream.Position;

                model.Fields.Flg.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Type.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Id.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.FlrNo.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Attr.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Px.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Py.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Pz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.W.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.H.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.D.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Prm0.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Prm1.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Prm2.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Prm3.SetValue(stream.Position, br.ReadBytes(1));

                list.Add(model);
            }

            return list;
        }

        public static List<AOTModel> ReadAOT(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadAOT(stream, header);
        }

        public static List<AOTModel> ReadAOT(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<AOTModel> list = new();

            stream.Seek(header.AOT.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.AOT.Count.Value; i++)
            {
                AOTModel model = new();

                model.Position = (nint)stream.Position;

                model.Fields.Flg.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Type.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Id.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.FlrNo.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Attr.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Px.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Py.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Pz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.W.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.H.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.D.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Prm0.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Prm1.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Prm2.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Prm3.SetValue(stream.Position, br.ReadBytes(1));

                list.Add(model);
            }

            return list;
        }

        public static List<TriggerModel> ReadTrigger(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadTrigger(stream, header);
        }

        public static List<TriggerModel> ReadTrigger(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<TriggerModel> list = new();

            stream.Seek(header.Trigger.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Trigger.Count.Value; i++)
            {
                TriggerModel model = new();

                model.Position = (nint)stream.Position;

                model.Fields.Flg.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Type.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Id.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.FlrNo.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Attr.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Px.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Py.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Pz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.W.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.H.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.D.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Prm0.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Prm1.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Prm2.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Prm3.SetValue(stream.Position, br.ReadBytes(1));

                list.Add(model);
            }

            return list;
        }

        public static List<PlayerModel> ReadPlayer(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadPlayer(stream, header);
        }

        public static List<PlayerModel> ReadPlayer(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<PlayerModel> list = new();

            stream.Seek(header.Player.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Player.Count.Value; i++)
            {
                PlayerModel model = new();

                model.Position = (nint)stream.Position;

                model.Fields.Px.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Py.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Pz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ay.SetValue(stream.Position, br.ReadBytes(4));

                list.Add(model);
            }

            return list;
        }

        public static List<RouteModel> ReadRoute(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadRoute(stream, header);
        }

        public static List<RouteModel> ReadRoute(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<RouteModel> list = new();

            stream.Seek(header.Route.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.Route.Count.Value; i++)
            {
                RouteModel model = new();

                model.Position = (nint)stream.Position;

                model.Fields.Flg.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Type.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Id.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.FlrNo.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Attr.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Px.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Py.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Pz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.W.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.H.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.D.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Prm0.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Prm1.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Prm2.SetValue(stream.Position, br.ReadBytes(1));
                model.Fields.Prm3.SetValue(stream.Position, br.ReadBytes(1));

                list.Add(model);
            }

            return list;
        }

        public static List<RouteTableModel> ReadRouteTable(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadRouteTable(stream, header);
        }

        public static List<RouteTableModel> ReadRouteTable(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<RouteTableModel> list = new();

            int count = (int)header.Route.Count.Value;

            if (header.RouteTable.Value == 0 || count == 0)
                return list;

            stream.Seek(header.RouteTable.Value, SeekOrigin.Begin);

            for (int i = 0; i < count * count; i++)
            {
                RouteTableModel model = new();

                model.Position = (nint)stream.Position;

                model.Fields.Value.SetValue(stream.Position, br.ReadBytes(1));

                list.Add(model);
            }

            return list;
        }

        public static List<EventCameraModel> ReadEventCamera(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadEventCamera(stream, header);
        }

        public static List<EventCameraModel> ReadEventCamera(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<EventCameraModel> list = new();

            stream.Seek(header.EventCamera.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.EventCamera.Count.Value; i++)
            {
                EventCameraModel camera = new();

                camera.Position = (nint)stream.Position;

                camera.Fields.Flg.SetValue(stream.Position, br.ReadBytes(2));
                camera.Fields.Type.SetValue(stream.Position, br.ReadBytes(2));
                camera.Fields.NxtNo.SetValue(stream.Position, br.ReadBytes(2));
                camera.Fields.KeyfN.SetValue(stream.Position, br.ReadBytes(2));

                for (int j = 0; j < 16; j++)
                {
                    EventCameraBlockModel model = new();

                    model.Position = (nint)stream.Position;
                    model.Header = camera;

                    model.Fields.Flg.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Frame.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Px.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Py.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Pz.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Ax.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Ay.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Az.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Pers.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.HidObj0.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj1.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj2.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj3.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj4.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj5.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj6.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidObj7.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidLgt0.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidLgt1.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidLgt2.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.HidLgt3.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.FogCol.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.FogNr.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.FogFr.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.LkFlg.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.LkNo.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.LkOno.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.NxtNo.SetValue(stream.Position, br.ReadBytes(2));
                    model.Fields.Lx.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Ly.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Lz.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Prm0.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Prm1.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Prm2.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Prm3.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Prm4.SetValue(stream.Position, br.ReadBytes(4));
                    model.Fields.Recp.SetValue(stream.Position, br.ReadBytes(4));

                    camera.Blocks.Add(model);
                }

                list.Add(camera);
            }

            return list;
        }

        public static List<EventLightModel> ReadEventLight(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadEventLight(stream, header);
        }

        public static List<EventLightModel> ReadEventLight(Stream stream, HeaderModel header)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            List<EventLightModel> list = new();

            stream.Seek(header.EventLight.Value, SeekOrigin.Begin);

            for (int i = 0; i < header.EventLight.Count.Value; i++)
            {
                EventLightModel model = new();

                model.Position = (nint)stream.Position;

                model.Fields.Flg.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Type.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Aspd.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.LkFlg.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.LkNo.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.LkOno.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Lsrc.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Px.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Py.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Pz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Lx.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ly.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Lz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Vx.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Vy.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Vz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Spc.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Dif.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Amb.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.R.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.G.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.B.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Nr.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Fr.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Iang.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Oang.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ax.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ay.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Az.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Mode.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ct0.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ct1.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ct2.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Ct3.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wpx.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wpy.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wpz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wvx.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wvy.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wvz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wspc.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wdif.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wamb.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wr.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wg.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wb.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wnr.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wfr.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wiang.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Woang.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Wax.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Way.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Waz.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Lkwkp.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Exp.SetValue(stream.Position, br.ReadBytes(4));
                model.Fields.Light.SetValue(stream.Position, br.ReadBytes(4));

                list.Add(model);
            }

            return list;
        }

        public static List<TextModel> ReadTexts(FileInfo file, HeaderModel header)
        {
            using FileStream stream = file.OpenReadShared();
            return ReadTexts(stream, header);
        }

        public static List<TextModel> ReadTexts(Stream stream, HeaderModel header)
        {
            List<TextModel> list = new();

            if (header.Text.Value == 0)
                return list;

            using BinaryReader br = new(stream, Encoding.Default, true);

            stream.Seek(header.Text.Value, SeekOrigin.Begin);

            int count = br.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                TextModel model = new();

                model.Position = (nint)stream.Position;

                model.Fields.Offset.SetValue(stream.Position, br.ReadBytes(4));
                model.Pointer.SetValue(model.Position, BitConverter.GetBytes(header.Text.Value + model.Fields.Offset.Value));

                list.Add(model);
            }

            uint nextSection = header.EventLight.Value != 0 ? header.EventLight.Value : header.Model.Value;

            for (int i = 0; i < list.Count; i++)
            {
                uint nextOffset = i < list.Count - 1 ? list[i + 1].Pointer.Value : nextSection;

                TextModel model = list[i];

                stream.Seek(model.Pointer.Value, SeekOrigin.Begin);

                model.Size = nextOffset - model.Pointer.Value;

                StringBuilder message = new();

                while (stream.Position < nextOffset)
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
                    // TODO: Create language tables non-US releases
                    FileInfo file = new(Path.Combine(AppContext.BaseDirectory, "Data\\Language\\US.tbl"));

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
            string key = value.ToString("X4");

            return CharacterMap.ContainsKey(key) ? CharacterMap[key] : $"[0x{key}]";
        }
    }
}