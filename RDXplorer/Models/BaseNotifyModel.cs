using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RDXplorer.Models
{
    public class BaseNotifyModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string name = null, params string[] properties)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            SendUpdateEvent(name, properties);

            return true;
        }

        public void SendUpdateEvent(string name, params string[] properties)
        {
            OnPropertyChanged(name);
            foreach (string property in properties)
                OnPropertyChanged(property);
        }

        public void SendUpdateEntryEvent() =>
            OnPropertyChanged("UpdateEntry");
    }
}