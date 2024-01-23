using RECVXFlagTool.Models;
using System.Collections.ObjectModel;

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
