using System;
using System.Globalization;
using System.Resources;

namespace Csizmazia.WpfDynamicUI.Localization
{
    public class ResxTranslationProvider
    {
        private readonly Type _resourceType;

        private readonly ResourceManager resourceManager;

        public ResxTranslationProvider(Type resourceType)
        {
            _resourceType = resourceType;
            resourceManager = new ResourceManager(resourceType);
        }

        public Type ResourceType
        {
            get { return _resourceType; }
        }

        public object Translate(string key, CultureInfo culture)
        {
            return resourceManager.GetString(key, culture);
        }
    }
}