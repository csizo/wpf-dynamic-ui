using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Csizmazia.WpfDynamicUI.MefComposition;

namespace Csizmazia.WpfDynamicUI.DataServices
{
    public class DynamicAssembly
    {
        private readonly Assembly _assembly;
        private readonly Type[] _assemblyTypes;


        public DynamicAssembly(Assembly assembly)
        {
            _assembly = assembly;
            _assemblyTypes = _assembly.GetTypes();
        }

        public void RegisterInMefContainer()
        {
            Container.Instance.RegisterAssembly(_assembly);
        }

        public EntityListModel<TContext, TEntity> CreateDynamicListModelInstance<TContext, TEntity>()
            where TContext : DbContext, new()
            where TEntity : class
        {
            Type entityType = typeof (TEntity);

            string typeName = string.Format("{0}EntityListModel", entityType.Name);

            Type type = _assemblyTypes.FirstOrDefault(t => t.Name == typeName);

            if (type == null)
                throw new InvalidOperationException("DynamicListModel not found: " + typeName);

            return (EntityListModel<TContext, TEntity>) Activator.CreateInstance(type, null);
        }
    }
}