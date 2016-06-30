using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.PropertyControlProviders;
using Image = System.Drawing.Image;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl
{
    public class SimplePropertyControlProviderHandler : IControlProviderHandler<PropertyInfo>
    {
        private static readonly Dictionary<Type, PropertyControlProvider> ProviderCache =
            new Dictionary<Type, PropertyControlProvider>
                {
                    {typeof (String), new StringControlProvider()},
                    {typeof (Int32), new IntegerControlProvider()},
                    {typeof (Int32?), new IntegerControlProvider()},
                    {typeof (Decimal), new DecimalControlProvider()},
                    {typeof (Decimal?), new DecimalControlProvider()},
                    {typeof (Double), new DoubleControlProvider()},
                    {typeof (Double?), new DoubleControlProvider()},
                    {typeof (TimeSpan), new TimeSpanControlProvider()},
                    {typeof (TimeSpan?), new TimeSpanControlProvider()},
                    {typeof (DateTime), new DateTimeControlProvider()},
                    {typeof (DateTime?), new DateTimeControlProvider()},
                    {typeof (Boolean), new BooleanControlProvider()},
                    {typeof (Boolean?), new BooleanControlProvider()},
                    {typeof (Image), new ImageControlProvider()},
                };

        #region IControlProviderHandler<PropertyInfo> Members

        public bool CanProvideControl(PropertyInfo parameter)
        {
            return ProviderCache.ContainsKey(parameter.PropertyType);
        }

        public Control ProvideControl(PropertyInfo parameter, object viewModel)
        {
            return ProviderCache[parameter.PropertyType].ProvideControl(parameter, viewModel);
        }

        #endregion
    }
}