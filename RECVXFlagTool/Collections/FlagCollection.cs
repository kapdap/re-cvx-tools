using System.Collections.ObjectModel;
using RECVXFlagTool.Models;

namespace RECVXFlagTool.Collections
{
	public class FlagCollection : ObservableCollection<FlagModel>
	{
		public string Name { get; set; }

		public FlagCollection() : base() { }
		public FlagCollection(string name) : base()
		{
			Name = name;
		}
	}
}
