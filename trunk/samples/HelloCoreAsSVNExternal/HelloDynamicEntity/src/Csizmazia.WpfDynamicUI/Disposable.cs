using System;

namespace Csizmazia.WpfDynamicUI
{
    public abstract class Disposable : IDisposable
    {
        private bool _disposed;

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// called when disposing
        /// </summary>
        protected virtual void OnDisposing()
        {
        }

        /// <summary>
        /// called when finalizing
        /// </summary>
        protected virtual void OnFinalizing()
        {
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    OnDisposing();
                else
                    OnFinalizing();

                _disposed = true;
            }
        }

        ~Disposable()
        {
            Dispose(false);
        }
    }
}