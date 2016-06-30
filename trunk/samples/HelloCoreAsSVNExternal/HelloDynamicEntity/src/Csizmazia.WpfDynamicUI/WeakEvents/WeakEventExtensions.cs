using System;
using System.ComponentModel;
using System.Reflection;
using Csizmazia.WpfDynamicUI.WeakEvents.EventConsumer;

//  Basic weak event management. 
// 
//  Weak allow objects to be garbage collected without having to unsubscribe
//  
//  Taken with some minor variations from:
//  http://diditwith.net/2007/03/23/SolvingTheProblemWithEventsWeakEventHandlers.aspx
//  
//  use as class.theEvent +=new EventHandler<EventArgs>(instance_handler).MakeWeak((e) => class.theEvent -= e);
//  MakeWeak extension methods take an delegate to unsubscribe the handler from the event
// 

namespace Csizmazia.WpfDynamicUI.WeakEvents
{
    /// <summary>
    /// Utilities for the weak event method
    /// </summary>
    public static class WeakEventExtensions
    {
        private static void CheckArgs(Delegate eventHandler, Delegate unregister)
        {
            if (eventHandler == null) throw new ArgumentNullException("eventHandler");
            if (eventHandler.Method.IsStatic || eventHandler.Target == null)
                throw new ArgumentException("Only instance methods are supported.", "eventHandler");
        }

        private static object GetWeakHandler(Type generalType, Type[] genericTypes, Type[] constructorArgTypes,
                                             object[] constructorArgs)
        {
            Type wehType = generalType.MakeGenericType(genericTypes);
            ConstructorInfo wehConstructor = wehType.GetConstructor(constructorArgTypes);
            if (wehConstructor == null)
                throw new NullReferenceException("cannot find constructor");
            return wehConstructor.Invoke(constructorArgs);
        }

        /// <summary>
        /// Makes a property change handler weak
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="unregister">The unregister.</param>
        /// <returns></returns>
        public static PropertyChangedEventHandler MakeWeak(this PropertyChangedEventHandler eventHandler,
                                                           UnregisterDelegate<PropertyChangedEventHandler> unregister)
        {
            CheckArgs(eventHandler, unregister);

            Type generalType = typeof (WeakPropertyChangeHandler<>);
            var genericTypes = new[] {eventHandler.Method.DeclaringType};
            var constructorTypes = new[]
                                       {
                                           typeof (PropertyChangedEventHandler),
                                           typeof (UnregisterDelegate<PropertyChangedEventHandler>)
                                       };
            var constructorArgs = new object[] {eventHandler, unregister};

            return
                ((IWeakPropertyChangedEventHandler)
                 GetWeakHandler(generalType, genericTypes, constructorTypes, constructorArgs)).Handler;
        }

        /// <summary>
        /// Makes a generic handler weak
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="unregister">The unregister.</param>
        /// <returns></returns>
        public static EventHandler<TEventArgs> MakeWeak<TEventArgs>(this EventHandler<TEventArgs> eventHandler,
                                                                    UnregisterDelegate<EventHandler<TEventArgs>>
                                                                        unregister) where TEventArgs : EventArgs
        {
            CheckArgs(eventHandler, unregister);

            Type generalType = typeof (WeakEventHandler<,>);
            var genericTypes = new[] {eventHandler.Method.DeclaringType, typeof (TEventArgs)};
            var constructorTypes = new[]
                                       {
                                           typeof (EventHandler<TEventArgs>),
                                           typeof (UnregisterDelegate<EventHandler<TEventArgs>>)
                                       };
            var constructorArgs = new object[] {eventHandler, unregister};

            return
                ((IWeakEventHandler<TEventArgs>)
                 GetWeakHandler(generalType, genericTypes, constructorTypes, constructorArgs)).Handler;
        }
    }
}