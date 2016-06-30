using System;
using System.Collections.Generic;
using System.Linq;
using Csizmazia.Collections;

namespace Csizmazia.WpfDynamicUI.WeakEvents.EventSource
{
    public class WeakEventSource<TEventArgs>
        where TEventArgs : EventArgs
    {
        private readonly WeakList<EventHandler<TEventArgs>> weakReferences =
            new WeakList<EventHandler<TEventArgs>>();


        public void RegisterEventHandler(EventHandler<TEventArgs> handler)
        {
            lock (weakReferences)
            {
                weakReferences.Clear(true);
                weakReferences.Add(handler);
            }
        }

        public void UnregisterEventHandler(EventHandler<TEventArgs> handler)
        {
            lock (weakReferences)
            {
                weakReferences.Clear(true);
                weakReferences.Remove(handler);
            }
        }

        public void RaiseEvent(object sender, TEventArgs e)
        {
            var handlers = new List<EventHandler<TEventArgs>>(weakReferences.Count);
            lock (weakReferences)
            {
                weakReferences.Clear(true);

                handlers.AddRange(weakReferences.Where(handler => handler != null));
            }

            foreach (var eventHandler in handlers)
            {
                eventHandler.Invoke(sender, e);
            }
        }
    }
}