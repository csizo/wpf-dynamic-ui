using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using Csizmazia.Tracing;

namespace Csizmazia.WpfDynamicUI.MefComposition
{
    /// <summary>
    /// Mef container
    /// </summary>
    public class Container : IContainer
    {
        public static readonly IContainer Instance = new Container();


        private static readonly AggregateCatalog Catalog = new AggregateCatalog();

        private volatile CompositionContainer _compositionContainer;


        private CompositionContainer CompositionContainer
        {
            get
            {
                if (_compositionContainer == null)
                {
                    lock (this)
                    {
                        if (_compositionContainer == null)
                        {
                            _compositionContainer = BuildCompositionContainer();
                        }
                    }
                }
                return _compositionContainer;
            }
        }

        #region IContainer Members

        public T GetExportedValue<T>() where T : class
        {
            return CompositionContainer.GetExportedValue<T>();
        }

        public IContainer ComposeParts<T>(T attributedPart) where T : class
        {
            if (attributedPart == null) throw new ArgumentNullException("attributedPart");


            CompositionContainer.ComposeParts(attributedPart);


            return this;
        }

        public IEnumerable<T> GetExportedValues<T>()
        {
            return CompositionContainer.GetExportedValues<T>();
        }

        public IEnumerable<Lazy<T>> GetExports<T>()
        {
            return CompositionContainer.GetExports<T>();
        }

        public IContainer ReleaseExports<T>(IEnumerable<Lazy<T>> exports)
        {
            CompositionContainer.ReleaseExports(exports);

            return this;
        }

        public IContainer ReleaseExport<T>(Lazy<T> export)
        {
            CompositionContainer.ReleaseExport(export);

            return this;
        }


        public IContainer RegisterAssembly(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            //add assembly if not already added
            if (Catalog.Catalogs.OfType<AssemblyCatalog>().All(ac => ac.Assembly != assembly))
                Catalog.Catalogs.Add(new AssemblyCatalog(assembly));

            return this;
        }
        public IContainer RegisterAssembly(params Assembly[] assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException("assemblies");

            foreach (var assembly in assemblies)
            {
                if (Catalog.Catalogs.OfType<AssemblyCatalog>().All(ac => ac.Assembly != assembly))
                    Catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            }


            return this;
        }

        public IContainer UnregisterAssembly(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            //find assemblies to remove
            var items = Catalog.Catalogs.OfType<AssemblyCatalog>().Where(ac => ac.Assembly == assembly);

            //remove assemblies from catalog
            foreach (var assemblyCatalog in items)
            {
                Catalog.Catalogs.Remove(assemblyCatalog);
            }

            return this;
        }

        public IContainer UnregisterAssembly(params Assembly[] assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException("assemblies");

            //find assemblies
            var items = Catalog.Catalogs.OfType<AssemblyCatalog>().Where(ac => assemblies.Contains(ac.Assembly));

            //remove assemblies from catalog
            foreach (var assemblyCatalog in items)
            {
                Catalog.Catalogs.Remove(assemblyCatalog);
            }


            return this;
        }

        #endregion

        private static CompositionContainer BuildCompositionContainer()
        {

            using (new MethodTracer<Container>("BuildCompositionContainer"))
            {
                //getting the location of the addon assemblies
                string location = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   where assembly == typeof(Container).Assembly
                                   select assembly.Location).First();

                string directory = Path.GetDirectoryName(location);

                //checking if directory has been found
                if (directory == null)
                {
                    //directory is null
                    throw new InvalidOperationException();
                }

                //adding the directory catalog to catalogs
                Catalog.Catalogs.Add(new DirectoryCatalog(directory));


                //creating the CompositionContainer with the parts in the catalog
                var compositionContainer = new CompositionContainer(Catalog);

                return compositionContainer;
            }
        }
    }
}