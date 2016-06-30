using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ReflectiveWpfTools
{
    public class LocalizationMarkupExtension : MarkupExtension
    {
        [ConstructorArgument("resourceType")]
        public Type ResourceType { get; set; }

        [ConstructorArgument("resourceKey")]
        public string ResourceKey { get; set; }

        [ConstructorArgument("defaultValue")]
        public string DefaultValue { get; set; }

        public LocalizationMarkupExtension()
        {
            
        }
        public LocalizationMarkupExtension(Type resourceType, string resourceKey, string defaultValue)
        {
            ResourceType = resourceType;
            ResourceKey = resourceKey;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// See <see cref="MarkupExtension.ProvideValue" />
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding("Value")
            {
                Source = new Localization.LocalizedString(ResourceType, ResourceKey, DefaultValue)
            };
            return binding.ProvideValue(serviceProvider);
        }

    }
}
