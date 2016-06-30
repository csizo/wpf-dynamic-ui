using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data.Entity;
using System.IO;
using System.Reflection;
using Csizmazia.Tracing;
using Microsoft.CSharp;

namespace Csizmazia.WpfDynamicUI.DataServices
{
    public class CodeGen
    {
        private readonly CodeCompileUnit _codeCompileUnit = new CodeCompileUnit();

        private readonly CodeNamespace _codeNamespace;
        private readonly DynamicListTypeBuilder _dynamicListTypeBuilder;

        public CodeGen()
        {
            _codeCompileUnit.AddReferences();
            _codeNamespace = _codeCompileUnit.DefineNamespace();

            _dynamicListTypeBuilder = new DynamicListTypeBuilder(this);
        }

        internal CodeCompileUnit CodeCompileUnit
        {
            get { return _codeCompileUnit; }
        }

        internal CodeNamespace CodeNamespace
        {
            get { return _codeNamespace; }
        }

        internal DynamicListTypeBuilder DynamicListTypeBuilder
        {
            get { return _dynamicListTypeBuilder; }
        }

        public void GenerateDynamicListModelType<TContext, TEntity>()
            where TContext : DbContext, new()
            where TEntity : class
        {
            GenerateDynamicListModelType<TContext>(typeof (TEntity));
        }

        public void GenerateDynamicListModelType<TContext>(params Type[] entityTypes)
            where TContext : DbContext, new()
        {
            foreach (Type entityType in entityTypes)
            {
                GenerateDynamicListModelType<TContext>(entityType);
            }
        }

        public void GenerateDynamicListModelType<TContext>(Type entityType)
            where TContext : DbContext, new()
        {
            //add reference to entity assembly
            _codeCompileUnit.AddReference(entityType);

            //build dynamic list model
            _dynamicListTypeBuilder.BuildDynamicListModel<TContext>(entityType);
        }

        public DynamicAssembly BuildTemporaryAssembly()
        {
            var provider = new CSharpCodeProvider();


            var parameters = new CompilerParameters
                                 {
                                     GenerateInMemory = true
                                 };


#if DEBUG
            provider.GenerateCodeFromCompileUnit(CodeCompileUnit, Console.Out, new CodeGeneratorOptions());
#endif

            CompilerResults results = provider.CompileAssemblyFromDom(parameters, CodeCompileUnit);
            if (results.Errors.Count > 0)
                Tracer<CodeGen>.Instance.Warn(() => results.Errors.ToString());

            Assembly assembly = results.CompiledAssembly;


            return new DynamicAssembly(assembly);
        }

        public DynamicAssembly BuildAssembly(string assemblyLocation)
        {
            var provider = new CSharpCodeProvider();


            var parameters = new CompilerParameters
                                 {
                                     GenerateInMemory = true,
                                     OutputAssembly = assemblyLocation
                                 };


#if DEBUG
            provider.GenerateCodeFromCompileUnit(CodeCompileUnit, Console.Out, new CodeGeneratorOptions());
#endif

            CompilerResults results = provider.CompileAssemblyFromDom(parameters, CodeCompileUnit);
            if (results.Errors.Count > 0)
                Tracer<CodeGen>.Instance.Warn(() => results.Errors.ToString());

            Assembly assembly = results.CompiledAssembly;


            return new DynamicAssembly(assembly);
        }

        public string BuildSourceCode(string sourceCodeLocation)
        {
            var provider = new CSharpCodeProvider();


            using (var sw = new StreamWriter(sourceCodeLocation))
            {
                provider.GenerateCodeFromCompileUnit(CodeCompileUnit, sw, new CodeGeneratorOptions());
            }

            using (var sr = new StreamReader(sourceCodeLocation))
            {
                return sr.ReadToEnd();
            }
        }
    }
}