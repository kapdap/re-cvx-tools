using System.Collections.Specialized;
using System.ComponentModel;
using RECVXFlagTool.Collections;
using RECVXFlagTool.Models;

namespace RECVXFlagTool.ViewModels
{
	public class FlagLogViewModel : MemoryViewModel
	{
		public FlagLogViewModel()
		{
			SetPropertyChangedEvents(Memory.DocumentFlags);
			SetPropertyChangedEvents(Memory.KilledFlags);
			SetPropertyChangedEvents(Memory.EventFlags);
			SetPropertyChangedEvents(Memory.ItemFlags);
			SetPropertyChangedEvents(Memory.MapFlags);

			Memory.DocumentFlags.CollectionChanged += SetPropertyChangedEvents;
			Memory.KilledFlags.CollectionChanged += SetPropertyChangedEvents;
			Memory.EventFlags.CollectionChanged += SetPropertyChangedEvents;
			Memory.ItemFlags.CollectionChanged += SetPropertyChangedEvents;
			Memory.MapFlags.CollectionChanged += SetPropertyChangedEvents;
		}

		private void SetPropertyChangedEvents(object sender, NotifyCollectionChangedEventArgs e)
		{
			foreach (FlagModel flag in e.NewItems)
				flag.PropertyChanged += PropertyChangedEvent;
		}

		private void SetPropertyChangedEvents(FlagCollection list)
		{
			foreach (FlagModel flag in list)
				flag.PropertyChanged += PropertyChangedEvent;
		}

		private void PropertyChangedEvent(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "Value")
				return;

			FlagModel flag = (FlagModel)sender;
			FlagLogModel log = new()
			{
				Flag = flag,
				Value = flag.Value
			};

			if (string.IsNullOrEmpty(log.Flag.Name) && flag.Flag != "Document")
			{
				log.Flag.Name = Memory.Room.Name;

				if (flag.Flag != "Map")
					log.Flag.Name = $"{log.Flag.Name} - ";
			}

			Memory.FlagLog.Add(log);
		}
	}
}