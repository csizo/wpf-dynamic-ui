using System;

namespace Csizmazia.WpfDynamicUI.WeakEvents.EventConsumer
{
    /// <summary>
    /// An interface for a weak event handler
    /// </summary>
    /// <typeparam name="TEventArg"></typeparam>
    public interface IWeakEventHandler<TEventArg> where TEventArg : EventArgs
    {
        EventHandler<TEventArg> Handler { get; }
    }
}