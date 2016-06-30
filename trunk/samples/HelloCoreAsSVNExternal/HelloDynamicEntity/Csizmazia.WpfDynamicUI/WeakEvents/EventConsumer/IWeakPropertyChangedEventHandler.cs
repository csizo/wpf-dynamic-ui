using System.ComponentModel;

namespace Csizmazia.WpfDynamicUI.WeakEvents.EventConsumer
{
    /// <summary>
    /// An interface for a weak event handler
    /// </summary>
    public interface IWeakPropertyChangedEventHandler
    {
        PropertyChangedEventHandler Handler { get; }
    }
}