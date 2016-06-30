using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;
using System.Xml;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ReflectiveWpfTools
{
    public static class XamlExportExtension
    {

        public static string ToXaml(this FrameworkElement frameworkElement)
        {
            var sb = new StringBuilder();
            var writer = XmlWriter.Create(sb, new XmlWriterSettings
            {
                Indent = true,
                ConformanceLevel = ConformanceLevel.Fragment,
                OmitXmlDeclaration = true
            });
            var mgr = new XamlDesignerSerializationManager(writer)
                          {
                              XamlWriterMode = XamlWriterMode.Expression
                          };
            System.Windows.Markup.XamlWriter.Save(frameworkElement, mgr);
            return sb.ToString();


        }
    }
}
