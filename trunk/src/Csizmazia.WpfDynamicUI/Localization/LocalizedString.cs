using System;

namespace Csizmazia.WpfDynamicUI.Localization
{
    public class LocalizedString : NotifyPropertyChanged
    {
        private readonly string _defaultValue;
        private readonly string _key;
        private readonly Type _resourceType;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedString"/> class.
        /// </summary>
        /// <param name="resourceType">The resource type</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue"> </param>
        public LocalizedString(Type resourceType, string key, string defaultValue)
        {
            _resourceType = resourceType;
            _key = key;
            _defaultValue = defaultValue;

            CultureManager.Instance.LanguageChanged += TranslationManagerLanguageChanged;
        }

        public object Value
        {
            get
            {
                object text = CultureManager.Instance.Translate(_resourceType, _key) ?? _defaultValue;
                return text;
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="LocalizedString"/> is reclaimed by garbage collection.
        /// </summary>
        ~LocalizedString()
        {
            CultureManager.Instance.LanguageChanged -= TranslationManagerLanguageChanged;
        }

        private void TranslationManagerLanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Value");
        }
    }
}