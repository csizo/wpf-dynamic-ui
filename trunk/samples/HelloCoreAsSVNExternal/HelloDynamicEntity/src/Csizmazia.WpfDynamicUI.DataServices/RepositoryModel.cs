using System.Data.Entity;
using Csizmazia.WpfDynamicUI.BusinessModel;

namespace Csizmazia.WpfDynamicUI.DataServices
{
    public abstract class RepositoryModel<TContext> : NavigationModel
        where TContext : DbContext, new()
    {
        private readonly Repository<TContext> _repository = new Repository<TContext>();

        protected Repository<TContext> Repository
        {
            get { return _repository; }
        }
    }
}