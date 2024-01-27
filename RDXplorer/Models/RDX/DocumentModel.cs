using RDXplorer.Formats.RDX;
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
                    SetField(ref _model, Reader.ReadModels(PathInfo, Header));
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
                    SetField(ref _motion, Reader.ReadMotions(PathInfo, Header));
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
                    SetField(ref _script, Reader.ReadScripts(PathInfo, Header));
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
                    SetField(ref _texture, Reader.ReadTextures(PathInfo, Header));
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
                    SetField(ref _camera, Reader.ReadCamera(PathInfo, Header));
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
                    SetField(ref _lighting, Reader.ReadLighting(PathInfo, Header));
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
                    SetField(ref _actor, Reader.ReadActor(PathInfo, Header));
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
                    SetField(ref _object, Reader.ReadObject(PathInfo, Header));
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
                    SetField(ref _item, Reader.ReadItem(PathInfo, Header));
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
                    SetField(ref _effect, Reader.ReadEffect(PathInfo, Header));
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
                    SetField(ref _boundary, Reader.ReadBoundary(PathInfo, Header));
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
                    SetField(ref _aot, Reader.ReadAOT(PathInfo, Header));
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
                    SetField(ref _trigger, Reader.ReadTrigger(PathInfo, Header));
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
                    SetField(ref _player, Reader.ReadPlayer(PathInfo, Header));
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
                    SetField(ref _event, Reader.ReadEvent(PathInfo, Header));
                return _event;
            }

            private set => SetField(ref _event, value);
        }

        public DocumentModel(FileInfo file)
        {
            PathInfo = file;
            Header = Reader.ReadHeader(file);
        }
    }
}