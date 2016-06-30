using System;

namespace Csizmazia.WpfDynamicUI.WeakEvents.EventConsumer
{
    /// <summary>
    /// A handler for an event that doesn't store a reference to the source
    /// handler must be a instance method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TEventArg"></typeparam>
    public class WeakEventHandler<T, TEventArg> : WeakEventHandlerGeneric<T, TEventArg, EventHandler<TEventArg>>,
                                                  IWeakEventHandler<TEventArg>
        where T : class
        where TEventArg : EventArgs
    {
        public WeakEventHandler(EventHandler<TEventArg> eventHandler,
                                UnregisterDelegate<EventHandler<TEventArg>> unregister)
            : base(eventHandler, unregister)
        {
        }
    }
}