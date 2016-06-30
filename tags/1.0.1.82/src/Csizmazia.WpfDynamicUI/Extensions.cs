using System;
using System.ComponentModel;
using System.Threading;

namespace Csizmazia.WpfDynamicUI
{
    public static class Extensions
    {
        public static T As<T>(this object item)
        {
            if (item == null) throw new ArgumentNullException("item");

            return (T) item;
        }

        /// <summary>
        /// Cast this delegate
        /// </summary>
        /// <typeparam name="T">Type of the result</typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T As<T>(this Delegate source) where T : class
        {
            return Cast(source, typeof (T)) as T;
        }


        public static EventHandler<TEventArgs> Cast<TEventArgs>(this Delegate @delegate) where TEventArgs : EventArgs
        {
            return (EventHandler<TEventArgs>) Cast(@delegate, typeof (EventHandler<TEventArgs>));
        }

        public static Delegate Cast(this Delegate source, Type targetType)
        {
            if (source == null)
                return null;


            Delegate[] delegates = source.GetInvocationList();
            if (delegates.Length == 1)
                return Delegate.CreateDelegate(targetType,
                                               delegates[0].Target, delegates[0].Method);

            var delegatesDest = new Delegate[delegates.Length];
            for (int nDelegate = 0; nDelegate < delegates.Length; nDelegate++)
                delegatesDest[nDelegate] = Delegate.CreateDelegate(targetType,
                                                                   delegates[nDelegate].Target,
                                                                   delegates[nDelegate].Method);
            return Delegate.Combine(delegatesDest);
        }


        public static EventHandler<PropertyChangedEventArgs> ToGenericEventHandler(
            this PropertyChangedEventHandler handler)
        {
            return new EventHandler<PropertyChangedEventArgs>(handler);
        }

        public static PropertyChangedEventHandler ToTypedPropertyChangedEventHandler(
            this EventHandler<PropertyChangedEventArgs> handler)
        {
            return new PropertyChangedEventHandler(handler);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rwLock"></param>
        /// <param name="action">Synchronized action</param>
        public static void WithWriteLock(this ReaderWriterLockSlim rwLock, Action action)
        {
            if (action == null) throw new ArgumentNullException("action");
            bool isEntered = false;
            try
            {
                if (!rwLock.IsWriteLockHeld)
                {
                    rwLock.EnterWriteLock();
                    isEntered = true;
                }

                action();
            }
            finally
            {
                if (isEntered)
                    rwLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rwLock"></param>
        /// <param name="action">Synchronized action</param>
        public static void WithReadLock(this ReaderWriterLockSlim rwLock, Action action)
        {
            if (action == null) throw new ArgumentNullException("action");
            bool isEntered = false;
            try
            {
                if (rwLock.IsWriteLockHeld == false && rwLock.IsUpgradeableReadLockHeld == false)
                {
                    rwLock.EnterUpgradeableReadLock();
                    isEntered = true;
                }

                action();
            }
            finally
            {
                if (isEntered)
                    rwLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rwLock"></param>
        /// <param name="func">Synchronized function</param>
        /// <returns></returns>
        public static T WithWriteLock<T>(this ReaderWriterLockSlim rwLock, Func<T> func)
        {
            if (func == null) throw new ArgumentNullException("func");
            bool isEntered = false;
            try
            {
                if (!rwLock.IsWriteLockHeld)
                {
                    rwLock.EnterWriteLock();
                    isEntered = true;
                }

                return func();
            }
            finally
            {
                if (isEntered)
                    rwLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rwLock"></param>
        /// <param name="func">Syncronized function</param>
        /// <returns></returns>
        public static T WithReadLock<T>(this ReaderWriterLockSlim rwLock, Func<T> func)
        {
            if (func == null) throw new ArgumentNullException("func");
            bool isEntered = false;
            try
            {
                if (rwLock.IsWriteLockHeld == false && rwLock.IsUpgradeableReadLockHeld == false)
                {
                    rwLock.EnterUpgradeableReadLock();
                    isEntered = true;
                }

                return func();
            }
            finally
            {
                if (isEntered)
                    rwLock.ExitUpgradeableReadLock();
            }
        }
    }
}