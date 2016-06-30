using System;
using System.Runtime.Serialization;

namespace Csizmazia.WpfDynamicUI
{
    /// <summary>
    /// Typed <see cref="System.WeakReference"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeakReference<T> : WeakReference
    {
        public WeakReference(T target)
            : base(target)
        {
        }

        public WeakReference(T target, bool trackResurrection)
            : base(target, trackResurrection)
        {
        }

        public WeakReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets or sets the weak target
        /// </summary>
        public new T Target
        {
            get { return (T) base.Target; }
            set { base.Target = value; }
        }
    }
}