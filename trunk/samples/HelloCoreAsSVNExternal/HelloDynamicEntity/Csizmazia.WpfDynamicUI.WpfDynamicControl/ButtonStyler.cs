using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Csizmazia.Discovering;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl
{
    internal static class ButtonStyler
    {
        private static readonly Dictionary<string, Style> ButtonStyleCache = new Dictionary<string, Style>();

        private static Style GetButtonStyle(string resourceKey)
        {
            return ButtonStyleCache.TryGetValue(resourceKey,
                                                () => Application.Current.TryFindResource(resourceKey) as Style);
        }

        private static List<string> TokenizeName(string text)
        {
            var tokens = new List<string>();

            var builder = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                if (Char.IsUpper(c) && builder.Length > 0)
                {
                    tokens.Add(builder.ToString());
                    builder.Clear();
                }
                builder.Append(c);
            }

            if (builder.Length > 0)
                tokens.Add(builder.ToString());

            return tokens;
        }

        private static bool TrySetButtonStyle(Button button, IEnumerable<string> nameTokens)
        {
            foreach (string nameToken in nameTokens)
            {
                string styleName = string.Format("Button{0}Style", nameToken);
                Style style = GetButtonStyle(styleName);
                if (style != null)
                {
                    button.SetValue(FrameworkElement.StyleProperty, style);
                    return true;
                }
            }

            return false;
        }

        public static void TrySetButtonStyle(this Button button, MethodInfo actionMethod)
        {
            List<string> nameTokens = TokenizeName(actionMethod.Name);
            //add default button style
            nameTokens.Add("");

            TrySetButtonStyle(button, nameTokens);
        }
    }
}