using System;

namespace Csizmazia.WpfDynamicUI.WeakEvents.EventConsumer
{
    /// <summary>
    /// A handler for an event that doesn't store a reference to the source
    /// handler must be a instance method
    /// </summary>
    /// <typeparam name="T">type of calling object</typeparam>
    /// <typeparam name="TEventArgs">type of event args</typeparam>
    /// <typeparam name="THandler">type of event handler</typeparam>
    public class WeakEventHandlerGeneric<T, TEventArgs, THandler>
        where T : class
        where TEventArgs : EventArgs
        where THandler : class
    {
        private readonly THandler m_Handler;
        private readonly OpenEventHandler m_OpenHandler;
        private readonly WeakReference m_TargetRef;
        private UnregisterDelegate<THandler> m_Unregister;

        public WeakEventHandlerGeneric(THandler eventHandler, UnregisterDelegate<THandler> unregister)
        {
            m_TargetRef = new WeakReference((eventHandler as Delegate).Target);
            m_OpenHandler =
                (OpenEventHandler)
                Delegate.CreateDelegate(typeof (OpenEventHandler), null, (eventHandler as Delegate).Method);
            m_Handler = CastDelegate(new LocalHandler(Invoke));
            m_Unregister = unregister;
        }

        /// <summary>
        /// Gets the handler.
        /// </summary>
        public THandler Handler
        {
            get { return m_Handler; }
        }

        private void Invoke(object sender, TEventArgs e)
        {
            var target = (T) m_TargetRef.Target;

            if (target != null)
                m_OpenHandler.Invoke(target, sender, e);
            else if (m_Unregister != null)
            {
                m_Unregister(m_Handler);
                m_Unregister = null;
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WeakEventHandler&lt;T,E&gt;"/> to <see cref="System.EventHandler&lt;E&gt;"/>.
        /// </summary>
        /// <param name="weh">The weh.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator THandler(WeakEventHandlerGeneric<T, TEventArgs, THandler> weh)
        {
            return weh.Handler;
        }

        /// <summary>
        /// Casts the delegate.
        /// Taken from
        /// http://jacobcarpenters.blogspot.com/2006/06/cast-delegate.html
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static THandler CastDelegate(Delegate source)
        {
            if (source == null) return null;

            Delegate[] delegates = source.GetInvocationList();
            if (delegates.Length == 1)
                return Delegate.CreateDelegate(typeof (THandler), delegates[0].Target, delegates[0].Method) as THandler;

            for (int i = 0; i < delegates.Length; i++)
                delegates[i] = Delegate.CreateDelegate(typeof (THandler), delegates[i].Target, delegates[i].Method);

            return Delegate.Combine(delegates) as THandler;
        }

        #region Nested type: LocalHandler

        private delegate void LocalHandler(object sender, TEventArgs e);

        #endregion

        #region Nested type: OpenEventHandler

        private delegate void OpenEventHandler(T @this, object sender, TEventArgs e);

        #endregion
    }
}