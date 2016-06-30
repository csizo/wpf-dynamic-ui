using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Csizmazia.WpfDynamicUI.DataServices
{
    public class Repository<TContext> : IDisposable
        where TContext : DbContext, new()
    {
        private readonly Lazy<TContext> _context = new Lazy<TContext>(() => new TContext());
        private bool _disposed;

        public TContext Context
        {
            get { return _context.Value; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public IQueryable<T> SelectAll<T>()
            where T : class
        {
            return Context.Set<T>();
        }

        public IQueryable<T> Select<T>(Expression<Func<T, bool>> condition)
            where T : class
        {
            return Context.Set<T>().Where(condition);
        }

        public T Delete<T>(T item)
            where T : class
        {
            return Context.Set<T>().Remove(item);
        }

        public T Create<T>()
            where T : class
        {
            return Context.Set<T>().Create();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_context.IsValueCreated)
                        _context.Value.Dispose();
                }
                _disposed = true;
            }
        }

        ~Repository()
        {
            Dispose(false);
        }
    }
}