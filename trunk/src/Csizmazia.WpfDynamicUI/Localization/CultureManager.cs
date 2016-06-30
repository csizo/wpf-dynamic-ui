using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Csizmazia.Discovering;
using Csizmazia.WpfDynamicUI.WeakEvents;

namespace Csizmazia.WpfDynamicUI.Localization
{
    public class CultureManager
    {
        private static volatile CultureManager _instance;
        private static readonly object InstanceLock = new Object();

        private static readonly Dictionary<Type, ResxTranslationProvider> TranslationProviders =
            new Dictionary<Type, ResxTranslationProvider>();

        private readonly List<EventHandler<EventArgs>> LanguageChangedEventSource = new List<EventHandler<EventArgs>>();

        private CultureManager()
        {
        }

        public CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
            set
            {
                if (value != Thread.CurrentThread.CurrentUICulture)
                {
                    Thread.CurrentThread.CurrentUICulture = value;
                    OnCultureChanged();
                }
            }
        }

        public static CultureManager Instance
        {
            get
            {
                if (_instance == null)
                    lock (InstanceLock)
                    {
                        if (_instance == null)
                            _instance = new CultureManager();
                    }
                return _instance;
            }
        }

        public event EventHandler<EventArgs> LanguageChanged
        {
            add { LanguageChangedEventSource.Add(value.MakeWeak(eh => LanguageChanged -= eh)); }
            remove { LanguageChangedEventSource.Remove(value); }
        }


        private void OnCultureChanged()
        {
            foreach (var eventHandler in LanguageChangedEventSource.ToList())
            {
                eventHandler.Invoke(this, EventArgs.Empty);
            }
        }

        public object Translate(Type resxType, string key)
        {
            ResxTranslationProvider provider = TranslationProviders.TryGetValue(resxType,
                                                                                () =>
                                                                                new ResxTranslationProvider(resxType));

            return provider.Translate(key, CurrentCulture);
        }
    }
}