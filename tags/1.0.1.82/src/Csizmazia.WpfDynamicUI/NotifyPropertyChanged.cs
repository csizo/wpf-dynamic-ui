using System.Collections.Generic;
using System.ComponentModel;

namespace Csizmazia.WpfDynamicUI
{
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        #region PropertyChangedEventArgsCache

        private static readonly Dictionary<string, PropertyChangedEventArgs> PropertyChangedEventArgsCache =
            new Dictionary<string, PropertyChangedEventArgs>();

        private static PropertyChangedEventArgs GetPropertyChangedEventArgs(string propertyName)
        {
            PropertyChangedEventArgs e;

            if (!PropertyChangedEventArgsCache.TryGetValue(propertyName, out e))
            {
                lock (PropertyChangedEventArgsCache)
                {
                    if (!PropertyChangedEventArgsCache.TryGetValue(propertyName, out e))
                    {
                        e = new PropertyChangedEventArgs(propertyName);
                        PropertyChangedEventArgsCache.Add(propertyName, e);
                    }
                }
            }

            return e;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected virtual void OnPropertyChanged(string propertyName, object before, object after)
        {
            PropertyChangedEventArgs eventArg = GetPropertyChangedEventArgs(propertyName);
            if (PropertyChanged != null)
                PropertyChanged(this, eventArg);
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName, null, propertyName);
        }
    }
}