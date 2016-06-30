namespace Csizmazia.WpfDynamicUI.WeakEvents.EventConsumer
{
    /// <summary>
    /// Delegate of an unsubscribe delegate
    /// </summary>
    public delegate void UnregisterDelegate<in THandler>(THandler eventHandler) where THandler : class;
}