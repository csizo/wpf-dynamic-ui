using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Controls;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ReflectiveWpfTools
{
    public class ReflectiveValidationMethodRule : ValidationRule
    {
        private readonly Func<string> _validationCallback;

        public ReflectiveValidationMethodRule(object viewModel, MethodInfo validationMethod)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            if (validationMethod == null) throw new ArgumentNullException("validationMethod");

            //build expression for viewModel.ValidationMethod();
            _validationCallback = Expression.Lambda<Func<string>>(
                Expression.Call(Expression.Constant(viewModel), validationMethod)).Compile();
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string error = _validationCallback();

            var validationResult = new ValidationResult(string.IsNullOrEmpty(error), error);
            return validationResult;
        }
    }
}