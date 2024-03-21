using RDXplorer.Extensions;
using RDXplorer.Formats.RDX;
using System.Collections.Generic;
using System.IO;

namespace RDXplorer.Models.RDX
{
    public class DocumentModel : BaseNotifyModel
    {
        private FileInfo _rdxPathInfo;
        public FileInfo RDXFileInfo
        {
            get => _rdxPathInfo;
            private set => SetField(ref _rdxPathInfo, value);
        }

        private FileInfo _prsFileInfo;
        public FileInfo PRSFileInfo
        {
            get => _prsFileInfo;
            private set => SetField(ref _prsFileInfo, value);
        }

        private MemoryStream _rdxStream;
        public MemoryStream RDXStream
        {
            get => _rdxStream;
            private set => SetField(ref _rdxStream, value);
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
                    SetField(ref _model, Reader.ReadModels(RDXStream, Header));
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
                    SetField(ref _motion, Reader.ReadMotions(RDXStream, Header));
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
                    SetField(ref _script, Reader.ReadScripts(RDXStream, Header));
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
                    SetField(ref _texture, Reader.ReadTextures(RDXStream, Header));
                return _texture;
            }

            private set => SetField(ref _texture, value);
        }

        private List<CameraHeaderModel> _camera;
        public List<CameraHeaderModel> Camera
        {
            get
            {
                if (_camera == null)
                    SetField(ref _camera, Reader.ReadCamera(RDXStream, Header));
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
                    SetField(ref _lighting, Reader.ReadLighting(RDXStream, Header));
                return _lighting;
            }

            private set => SetField(ref _lighting, value);
        }

        private List<EnemyModel> _enemy;
        public List<EnemyModel> Enemy
        {
            get
            {
                if (_enemy == null)
                    SetField(ref _enemy, Reader.ReadEnemy(RDXStream, Header));
                return _enemy;
            }

            private set => SetField(ref _enemy, value);
        }

        private List<ObjectModel> _object;
        public List<ObjectModel> Object
        {
            get
            {
                if (_object == null)
                    SetField(ref _object, Reader.ReadObject(RDXStream, Header));
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
                    SetField(ref _item, Reader.ReadItem(RDXStream, Header));
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
                    SetField(ref _effect, Reader.ReadEffect(RDXStream, Header));
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
                    SetField(ref _boundary, Reader.ReadBoundary(RDXStream, Header));
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
                    SetField(ref _aot, Reader.ReadAOT(RDXStream, Header));
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
                    SetField(ref _trigger, Reader.ReadTrigger(RDXStream, Header));
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
                    SetField(ref _player, Reader.ReadPlayer(RDXStream, Header));
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
                    SetField(ref _event, Reader.ReadEvent(RDXStream, Header));
                return _event;
            }

            private set => SetField(ref _event, value);
        }

        private List<TextModel> _text;
        public List<TextModel> Text
        {
            get
            {
                if (_text == null)
                    SetField(ref _text, Reader.ReadTexts(RDXStream, Header));
                return _text;
            }

            private set => SetField(ref _text, value);
        }

        public DocumentModel(FileInfo file)
        {
            RDXFileInfo = file;
            RDXStream = new();

            using (FileStream fileStream = file.OpenReadShared())
                fileStream.CopyTo(RDXStream);

            Header = Reader.ReadHeader(RDXStream);
        }

        public DocumentModel(FileInfo file, FileInfo prs)
        {
            RDXFileInfo = file;
            PRSFileInfo = prs;
            RDXStream = new();

            using (FileStream fileStream = file.OpenReadShared())
                fileStream.CopyTo(RDXStream);
            
            Header = Reader.ReadHeader(RDXStream);
        }
    }
}