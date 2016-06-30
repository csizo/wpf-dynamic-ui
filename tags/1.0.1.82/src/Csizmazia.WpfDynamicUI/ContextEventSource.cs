using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Csizmazia.WpfDynamicUI
{
    /// <summary>
    ///Dispatcher event source
    /// <para>The event handler is synchronized to the SynchronizationContext at the event registration time so the event registration should be called from the UI thread (binding, etc) and not from a background job...</para>
    /// <para>The ContextEventSource can handle any event declaration</para>
    /// </summary>
    /// <example>
    /// <code>
    /// class ClassWithEventSource
    /// {
    ///     private ContextEventSource&lt;PropertyChangedEventArgs&gt; _propertyChangedEvent = new ContextEventSource&lt;PropertyChangedEventArgs&gt;();
    ///
    ///     public event PropertyChangedEventHandler PropertyChanged { add { _propertyChangedEvent.AddHandler(value); } remove { _propertyChangedEvent.RemoveHandler(value); } }
    ///
    ///     protected virtual void OnPropertyChanged(string propertyName)
    ///     {
    ///     _propertyChangedEvent.InvokeHandler(this, GetPropertyChangedEventArg(propertyName));
    ///     }
    /// }
    /// class ClassUsingEventSource
    /// {
    ///     ClassWithEventSource _classWithEventSource = new ClassWithEventSource();
    ///     public ClassUsingEventSource()
    ///     {
    ///         //call this from UI thread
    ///         _classWithEventSource.PropertyChanged += new PropertyChangedEventHandler...
    /// 
    ///         //job
    ///         ThreadPool.QueueWorkItem((object state)=> { _classWithEventSource.IDistillerModule = "Value" });
    ///     }
    ///     
    /// }
    /// </code>
    /// </example>
    /// <typeparam name="TEventArgs"></typeparam>
    public sealed class ContextEventSource<TEventArgs> where TEventArgs : EventArgs
    {
        private readonly List<EventHandlerItem> _eventHandlers = new List<EventHandlerItem>();


        public int Count
        {
            get
            {
                lock (_eventHandlers)
                {
                    return _eventHandlers.Count;
                }
            }
        }

        public void AddHandlers(ContextEventSource<TEventArgs> eventSource)
        {
            lock (_eventHandlers)
            {
                _eventHandlers.AddRange(eventSource._eventHandlers);
            }
        }

        public int RemoveHandlers(Func<EventHandler<TEventArgs>, bool> predicate)
        {
            lock (_eventHandlers)
            {
                return _eventHandlers.RemoveAll(ev => predicate(ev.Handler));
            }
        }

        /// <summary>
        /// Invoke the handler respectful to actual handler synchronization context (if any)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public ContextEventSource<TEventArgs> InvokeHandler(object sender, TEventArgs e)
        {
            var eventHandlerItems = new List<EventHandlerItem>(_eventHandlers.Count);
            lock (_eventHandlers)
            {
                eventHandlerItems.AddRange(_eventHandlers);
            }

            foreach (EventHandlerItem eventHandlerItem in eventHandlerItems)
            {
                eventHandlerItem.InvokeHandler(sender, e);
            }

            return this;
        }

        #region RemoveHandler

        public ContextEventSource<TEventArgs> RemoveHandler(Delegate handler)
        {
            return RemoveHandler(handler.As<EventHandler<TEventArgs>>());
        }

        public ContextEventSource<TEventArgs> RemoveHandler(EventHandler<TEventArgs> handler)
        {
            lock (_eventHandlers)
            {
                _eventHandlers.RemoveAll(h => h.Handler == handler);
            }
            return this;
        }

        #endregion

        #region AddHandler

        public ContextEventSource<TEventArgs> AddHandler(Delegate handler)
        {
            return AddHandler(handler.As<EventHandler<TEventArgs>>());
        }

        public ContextEventSource<TEventArgs> AddHandler(EventHandler<TEventArgs> handler)
        {
            lock (_eventHandlers)
            {
                _eventHandlers.Add(new EventHandlerItem {Handler = handler});
            }
            return this;
        }

        #endregion

        #region Nested type: EventHandlerItem

        [DebuggerDisplay("{Handler.Target.GetType().Name}")]
        private class EventHandlerItem
        {
            private readonly SynchronizationContext _synchronizationContext;
            public EventHandler<TEventArgs> Handler;

            public EventHandlerItem()
            {
                _synchronizationContext = SynchronizationContext.Current;
            }


            public void InvokeHandler(object sender, TEventArgs e)
            {
                if (_synchronizationContext == null || SynchronizationContext.Current == _synchronizationContext)
                    Handler(sender, e);
                else
                {
                    //Send or Post message on the SynchronizationContext
                    if (_synchronizationContext.IsWaitNotificationRequired())
                        _synchronizationContext.Post(InvokeHandlerOnContext,
                                                     new InvokeHandlerOnContextState(Handler, sender, e));
                    else
                        _synchronizationContext.Send(InvokeHandlerOnContext,
                                                     new InvokeHandlerOnContextState(Handler, sender, e));
                }
            }

            private static void InvokeHandlerOnContext(object state)
            {
                var invokeActionOnContextState = state as InvokeHandlerOnContextState;
                if (invokeActionOnContextState == null)
                {
                    throw new ArgumentException("state must be InvokeHandlerOnContextState");
                }
                invokeActionOnContextState.Handler(invokeActionOnContextState.Sender,
                                                   invokeActionOnContextState.EventArg);
            }
        }

        #endregion

        #region Nested type: InvokeHandlerOnContextState

        private class InvokeHandlerOnContextState
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly TEventArgs _eventArg;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly EventHandler<TEventArgs> _handler;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly object _sender;

            public InvokeHandlerOnContextState(EventHandler<TEventArgs> handler, object sender, TEventArgs eventArg)
            {
                _handler = handler;
                _eventArg = eventArg;
                _sender = sender;
            }

            public EventHandler<TEventArgs> Handler
            {
                get { return _handler; }
            }

            public object Sender
            {
                get { return _sender; }
            }

            public TEventArgs EventArg
            {
                get { return _eventArg; }
            }
        }

        #endregion
    }
}