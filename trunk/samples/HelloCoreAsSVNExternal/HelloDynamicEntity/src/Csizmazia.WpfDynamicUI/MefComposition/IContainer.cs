using System;
using System.Collections.Generic;
using System.Reflection;

namespace Csizmazia.WpfDynamicUI.MefComposition
{
    public interface IContainer
    {
        IContainer ComposeParts<T>(T attributedPart) where T : class;
        T GetExportedValue<T>() where T : class;
        IEnumerable<T> GetExportedValues<T>();

        IEnumerable<Lazy<T>> GetExports<T>();

        IContainer ReleaseExports<T>(IEnumerable<Lazy<T>> exports);

        IContainer ReleaseExport<T>(Lazy<T> export);

        IContainer RegisterAssembly(Assembly assembly);

        IContainer RegisterAssembly(params Assembly[] assemblies);

        IContainer UnregisterAssembly(Assembly assembly);

        IContainer UnregisterAssembly(params Assembly[] assemblies);
    }
}