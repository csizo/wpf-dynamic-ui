using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Csizmazia.Discovering;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.CustomControls;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ModelDiscovery;
using Microsoft.Windows.Controls;
using RichTextBox = Microsoft.Windows.Controls.RichTextBox;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.PropertyControlProviders
{
    public class StringControlProvider : SimplePropertyControlProvider
    {
        public override Control ProvideControl(PropertyInfo propertyInfo, object viewModel)
        {
            Binding binding = propertyInfo.GetBinding();


            var uiHintAttribute = propertyInfo.GetAttribute<UIHintAttribute>();

            //checking if item is for filter area...
            bool isFilterProperty = propertyInfo.IsFilterPropertyForUserInterface();
            if (isFilterProperty && uiHintAttribute == null)
            {
                //override display to DisplayDelayedBindingTextBox
                uiHintAttribute = new UIHintAttribute(UIHints.DisplayDelayedBindingTextBox);
            }

            Control control;
            if (uiHintAttribute == null)
                control = ProvideDefaultControl(binding, propertyInfo);
            else
            {
                switch (uiHintAttribute.UIHint)
                {
                    case UIHints.DisplayRichTextBox:
                        control = ProvideRichTextBoxControl(binding, propertyInfo);
                        break;
                    case UIHints.DisplayMultiLineTextBox:
                        control = ProvideDefaultControl(binding, propertyInfo, true);
                        break;
                    case UIHints.DisplayDelayedBindingTextBox:
                        control = ProvideDelayedBindingTextBoxControl(binding, propertyInfo);
                        break;
                    case UIHints.DisplayHyperlink:
                        control = ProvideHyperLinkControl(binding, propertyInfo, viewModel);
                        break;
                    case UIHints.DisplayWebBrowser:
                        control = ProvideWebBrowserControl(binding, propertyInfo, viewModel);
                        break;
                    default:
                        //warn default case
                        control = ProvideDefaultControl(binding, propertyInfo);
                        break;
                }
            }


            return control;
        }
        private static Control ProvideHyperLinkControl(Binding binding, PropertyInfo propertyInfo, object viewModel)
        {
            var control = new ContentControl();


            var text = (string)propertyInfo.GetValue(viewModel, null);
            var ln = new Run(text);
            var hyperlink = new Hyperlink(ln);

            hyperlink.SetBinding(Hyperlink.NavigateUriProperty, binding);



            hyperlink.RequestNavigate += (o, e) =>
                                 {
                                     Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                                     e.Handled = true;

                                 };
            hyperlink.Unloaded += (o, e) =>
                                    {
                                        var hl = o as Hyperlink;
                                        if (hl == null)
                                            return;

                                        BindingOperations.ClearAllBindings(hl);
                                    };

            control.SetValue(ContentControl.ContentProperty, hyperlink);
            return control;
        }

        private static ContentControl ProvideWebBrowserControl(Binding binding, PropertyInfo propertyInfo, object viewModel)
        {
            var control = new ContentControl();
            var browser = new WebBrowser();

            var vm = (INotifyPropertyChanged)viewModel;
            if (vm != null)
            {
                vm.PropertyChanged += (o, e) =>
                                          {
                                              if (e.PropertyName == propertyInfo.Name)
                                              {
                                                  var url = (string)propertyInfo.GetValue(o, null);
                                                  browser.Navigate(url);
                                              }
                                          };

                var defaultUrl = (string)propertyInfo.GetValue(viewModel, null);
                if (string.IsNullOrEmpty(defaultUrl))
                    defaultUrl = "about:Blank";

                browser.Navigate(defaultUrl);

            }
            else
            {
                browser.Navigate("http://wpfdynamicui.codeplex.com");
            }

            control.SetValue(ContentControl.ContentProperty, browser);
            return control;
        }

        private static RichTextBox ProvideRichTextBoxControl(Binding binding, PropertyInfo propertyInfo)
        {
            var editableAttribute = propertyInfo.GetAttribute<EditableAttribute>();


            var control = new RichTextBox();
            control.SetBinding(RichTextBox.TextProperty, binding);
            control.TextFormatter = new PlainTextFormatter();

            //add formatBar
            var formatBar = new RichTextBoxFormatBar();
            RichTextBoxFormatBarManager.SetFormatBar(control, formatBar);


            control.IsReadOnly = propertyInfo.IsControlReadonly();

            return control;
        }

        private static DelayedBindingTextBox ProvideDelayedBindingTextBoxControl(Binding binding,
                                                                                 PropertyInfo propertyInfo)
        {
            var stringLengthAttribute = propertyInfo.GetAttribute<StringLengthAttribute>();
            var editableAttribute = propertyInfo.GetAttribute<EditableAttribute>();

            var control = new DelayedBindingTextBox();
            control.SetBinding(TextBox.TextProperty, binding);

            if (stringLengthAttribute != null)
            {
                control.SetValue(TextBox.MaxLengthProperty, stringLengthAttribute.MaximumLength);
            }


            control.IsReadOnly = propertyInfo.IsControlReadonly();

            return control;
        }

        private static Control ProvideDefaultControl(Binding binding, PropertyInfo propertyInfo,
                                                     bool isMultiLineTextBox = false)
        {
            var stringLengthAttribute = propertyInfo.GetAttribute<StringLengthAttribute>();
            var editableAttribute = propertyInfo.GetAttribute<EditableAttribute>();


            var control = new WatermarkTextBox();
            control.SetBinding(TextBox.TextProperty, binding);

            if (stringLengthAttribute != null)
            {
                if (stringLengthAttribute.MaximumLength > 100)
                    isMultiLineTextBox = true;

                control.SetValue(TextBox.MaxLengthProperty, stringLengthAttribute.MaximumLength);
            }

            //set textbox to multiline textbox
            if (isMultiLineTextBox)
            {
                control.TextWrapping = TextWrapping.Wrap;
                control.AcceptsReturn = true;
                control.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                control.Height = 100;
            }


            control.SetBinding(WatermarkTextBox.WatermarkProperty, propertyInfo.GetDisplayPromptBinding());


            control.IsReadOnly = propertyInfo.IsControlReadonly();

            return control;
        }
    }
}