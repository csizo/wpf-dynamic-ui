using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ReflectiveWpfTools;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.PropertyControlProviders
{
    public class ImageControlProvider : SimplePropertyControlProvider
    {
        private static readonly ImageToBitmapImageConverter Converter = new ImageToBitmapImageConverter();

        public override Control ProvideControl(PropertyInfo propertyInfo, object viewModel)
        {
            Binding binding = propertyInfo.GetBinding();
            binding.Mode = BindingMode.OneWay;
            binding.Converter = Converter;


            var control = new ContentControl();

            var image = new Image();
            image.SetBinding(Image.SourceProperty, binding);

            control.SetValue(ContentControl.ContentProperty, image);

            return control;
        }
    }
}