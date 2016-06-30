using System;
using System.Collections.Generic;

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
    }
}