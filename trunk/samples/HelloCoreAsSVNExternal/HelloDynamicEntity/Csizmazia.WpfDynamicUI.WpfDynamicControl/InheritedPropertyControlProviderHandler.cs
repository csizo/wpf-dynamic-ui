using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using Csizmazia.Collections;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.PropertyControlProviders;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl
{
    public class InheritedPropertyControlProviderHandler : IControlProviderHandler<PropertyInfo>
    {
        private static readonly Dictionary<Type, PropertyControlProvider> ProviderCache =
            new Dictionary<Type, PropertyControlProvider>
                {
                    {typeof (Control), new WpfControlControlProvider()},
                    {typeof (IPagedQueryable), new PagedQueryableControlProvider()},
                    {typeof (IEnumerable), new ItemsControlProvider()},
                    {typeof (INotifyPropertyChanged), new ModelControlProvider()},
                };

        #region IControlProviderHandler<PropertyInfo> Members

        public bool CanProvideControl(PropertyInfo propertyInfo)
        {
            return ProviderCache.Any(kv => kv.Key.IsAssignableFrom(propertyInfo.PropertyType));
        }

        public Control ProvideControl(PropertyInfo propertyInfo, object viewModel)
        {
            PropertyControlProvider controlProvider =
                ProviderCache.Where(kv => kv.Key.IsAssignableFrom(propertyInfo.PropertyType)).Select(kv => kv.Value).
                    First();
            return controlProvider.ProvideControl(propertyInfo, viewModel);
        }

        #endregion
    }
}