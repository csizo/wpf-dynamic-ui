using System.ComponentModel;

namespace Csizmazia.WpfDynamicUI.WeakEvents.EventConsumer
{
    /// <summary>
    /// A handler for an event that doesn't store a reference to the source
    /// handler must be a instance method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeakPropertyChangeHandler<T> :
        WeakEventHandlerGeneric<T, PropertyChangedEventArgs, PropertyChangedEventHandler>,
        IWeakPropertyChangedEventHandler
        where T : class
    {
        public WeakPropertyChangeHandler(PropertyChangedEventHandler eventHandler,
                                         UnregisterDelegate<PropertyChangedEventHandler> unregister)
            : base(eventHandler, unregister)
        {
        }
    }
}