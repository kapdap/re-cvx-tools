﻿using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class ObjectViewModel : PageViewModel<ObjectViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (ObjectModel item in AppViewModel.RDXDocument.Object)
                Entries.Add(new ObjectViewModelEntry(item));
        }
    }

    public class ObjectViewModelEntry(ObjectModel model) : PageViewModelEntry<ObjectModel>(model) { }
}