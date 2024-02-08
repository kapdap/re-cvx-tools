using RECVXFlagTool.Collections;
using RECVXFlagTool.Models;
using RECVXFlagTool.Models.Base;
using System.Windows.Data;

namespace RECVXFlagTool
{
    public class GameMemory : BaseNotifyModel
    {
        public GameModel Game { get; } = new GameModel();
        public RoomModel Room { get; } = new RoomModel();

        private object _documentFlagsLock = new();
        public FlagCollection DocumentFlags { get; } = new FlagCollection("Document");

        private object _killedFlagsLock = new();
        public FlagCollection KilledFlags { get; } = new FlagCollection("Killed");

        private object _eventFlagsLock = new();
        public FlagCollection EventFlags { get; } = new FlagCollection("Event");

        private object _itemFlagsLock = new();
        public FlagCollection ItemFlags { get; } = new FlagCollection("Item");

        private object _mapFlagsLock = new();
        public FlagCollection MapFlags { get; } = new FlagCollection("Map");

        private object _flagLogLock = new();
        public FlagLogCollection FlagLog { get; set; } = new FlagLogCollection();

        public GameMemory()
        {
            BindingOperations.EnableCollectionSynchronization(DocumentFlags, _documentFlagsLock);
            BindingOperations.EnableCollectionSynchronization(KilledFlags, _killedFlagsLock);
            BindingOperations.EnableCollectionSynchronization(EventFlags, _eventFlagsLock);
            BindingOperations.EnableCollectionSynchronization(ItemFlags, _itemFlagsLock);
            BindingOperations.EnableCollectionSynchronization(MapFlags, _mapFlagsLock);
            BindingOperations.EnableCollectionSynchronization(FlagLog, _flagLogLock);
        }

        public void ClearFlags()
        {
            DocumentFlags.Clear();
            KilledFlags.Clear();
            EventFlags.Clear();
            ItemFlags.Clear();
            MapFlags.Clear();
            FlagLog.Clear();
        }
    }
}