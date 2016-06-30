using System;
using System.Diagnostics;

namespace Csizmazia.Tracing
{
    public class Tracer<T> : ITracer
    {
        // ReSharper disable StaticFieldInGenericType
        private static readonly TraceSource TraceSource = new TraceSource(typeof (T).FullName);

        public static Tracer<T> Instance
        {
            get { return new Tracer<T>(); }
        }

        // ReSharper restore StaticFieldInGenericType

        #region ITracer Members

        public void Verbose(Func<string> message)
        {
#if TRACE
            TraceSource.TraceEvent(TraceEventType.Verbose, 0, message());
#endif
#if DEBUG
            Debug.WriteLine("[Verbose]" + message());
#endif
        }


        public void Info(Func<string> message)
        {
#if TRACE
            TraceSource.TraceEvent(TraceEventType.Information, 0, message());
#endif
#if DEBUG
            Debug.WriteLine("[Info]" + message());
#endif
        }

        public void Warn(Func<string> message)
        {
#if TRACE
            TraceSource.TraceEvent(TraceEventType.Warning, 0, message());
#endif
#if DEBUG
            Debug.WriteLine("[Warn]" + message());
#endif
        }

        public void Error(Func<string> message)
        {
#if TRACE
            TraceSource.TraceEvent(TraceEventType.Error, 0, message());
#endif
#if DEBUG
            Debug.WriteLine("[Error]" + message());
#endif
        }

        public void Error(Func<string> message, Exception ex)
        {
#if TRACE
            TraceSource.TraceEvent(TraceEventType.Error, 0, message());
            TraceSource.TraceData(TraceEventType.Error, 0, ex);
#endif
#if DEBUG
            Debug.WriteLine("[Error]" + message());
#endif
        }

        public void Critical(Func<string> message, Exception ex)
        {
#if TRACE
            TraceSource.TraceEvent(TraceEventType.Critical, 0, message());
            TraceSource.TraceData(TraceEventType.Critical, 0, ex);
#endif
#if DEBUG
            Debug.WriteLine("[Critical]" + message());
#endif
        }

        #endregion
    }
}