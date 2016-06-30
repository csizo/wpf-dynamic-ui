using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Csizmazia.WpfDynamicUI.BusinessModel;
using LinqKit;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Container = Csizmazia.WpfDynamicUI.MefComposition.Container;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel.DynamicCode
{
    public class DynamicCodeBuilder : NavigationModel
    {


        private static readonly string[] _SourceCodeTypes = new[] { "C#", "VB.NET" };

        [UIHint(UIHints.DisplayComboBox)]
        public IEnumerable<string> SourceCodeTypes
        {
            get { return _SourceCodeTypes; }
        }

        [Display(AutoGenerateField = false)]
        public string SelectedSourceCodeTypesItem { get; set; }


        //public bool GenerateInMemory { get; set; }

        [UIHint(UIHints.DisplayMultiLineTextBox, "Wpf", UIHints.DisplayParameters.Height, 300.0)]
        public string SourceCode { get; set; }

        [Editable(false)]
        [UIHint(UIHints.DisplayMultiLineTextBox, "Wpf", UIHints.DisplayParameters.Height, 150.0)]
        public string Result { get; private set; }


        [Display(AutoGenerateField = false)]
        public bool CanCompileAndRegisterDynamicAssembly { get; private set; }

        public void LoadSampleCode()
        {
            SelectedSourceCodeTypesItem = "C#";

            string[] names = typeof(DynamicCodeBuilder).Assembly.GetManifestResourceNames();
            string SampleCodeName = "DynamicHelloWorldClass.cs";

            using (Stream stream = typeof(DynamicCodeBuilder).Assembly.
                GetManifestResourceStream(names.FirstOrDefault(n => n.Contains(SampleCodeName))))
            {
                using (var sr = new StreamReader(stream))
                {
                    SourceCode = sr.ReadToEnd();
                }
            }
        }

        [Display(AutoGenerateField = true, Name = "Compile", Description = "Compile and register dynamic assembly")]
        public void CompileAndRegisterDynamicAssembly()
        {
            CodeDomProvider provider = null;
            switch (SelectedSourceCodeTypesItem)
            {
                case "C#":
                    provider = new CSharpCodeProvider();
                    break;
                case "VB.NET":
                    provider = new VBCodeProvider();
                    break;
            }


            var parameters = new CompilerParameters
                                 {
                                     GenerateInMemory = true
                                 };

            parameters.ReferencedAssemblies.Add(typeof(int).Assembly.Location);
            parameters.ReferencedAssemblies.Add(typeof(INotifyPropertyChanged).Assembly.Location); //System.dll
            //parameters.ReferencedAssemblies.Add(typeof(DbContext).Assembly.Location);
            parameters.ReferencedAssemblies.Add(typeof(DynamicCodeBuilder).Assembly.Location);
            parameters.ReferencedAssemblies.Add(typeof(BusinessApplication).Assembly.Location);
            parameters.ReferencedAssemblies.Add(typeof(DisplayAttribute).Assembly.Location);
            parameters.ReferencedAssemblies.Add(typeof(Expression<>).Assembly.Location);
            parameters.ReferencedAssemblies.Add(typeof(Func<>).Assembly.Location);
            parameters.ReferencedAssemblies.Add(typeof(Linq).Assembly.Location);

            CompilerResults result = provider.CompileAssemblyFromSource(parameters, SourceCode
                /*.Split(new[] { '\r', '\n' })*/);


            var sb = new StringBuilder();

            foreach (string output in result.Output.OfType<string>())
            {
                sb.AppendLine(output);
            }


            if (result.Errors.Count == 0)
            {
                Assembly asm = result.CompiledAssembly;
                Container.Instance.RegisterAssembly(asm);

                BusinessApplication.Instance.ShowPopup("Code Successfully compiled and registered");

                sb.AppendLine("WpfDynamicUI: Code Successfully compiled and registered");
            }

            Result = sb.ToString();
        }

        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName == "SourceCode")
                CanCompileAndRegisterDynamicAssembly = !string.IsNullOrEmpty(SourceCode);

            base.OnPropertyChanged(propertyName, before, after);
        }

        protected override void OnOpened()
        {

            this.HelpText =
                "Enter source code, or load the sample source code. Compile the code into WpfDynamicUI loadable module!";

            SelectedSourceCodeTypesItem = "C#";

            BusinessApplication.Instance.ShowPopup("Enter source code ");
            base.OnOpened();
        }

        [Display(AutoGenerateField = true, Name = "Dynamic code builder")]
        public static void LoadPagedRiport(WorkspaceModel workspace)
        {
            BusinessApplication.Instance.OpenModel(() => new DynamicCodeBuilder());
        }
    }
}