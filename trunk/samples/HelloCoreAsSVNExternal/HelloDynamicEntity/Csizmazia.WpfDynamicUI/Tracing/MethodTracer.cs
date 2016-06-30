using System;
using System.Diagnostics;

namespace Csizmazia.Tracing
{
    public class MethodTracer<T> : Tracer<T>, IDisposable
    {
        private readonly string _methodname;


        public MethodTracer()
        {
            Trace.Indent();
        }

        public MethodTracer(string methodname)
        {
            Trace.Indent();

            _methodname = methodname;
            if (!string.IsNullOrEmpty(methodname))
            {
                Verbose(() => string.Format(">>{0}()", methodname));
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!string.IsNullOrEmpty(_methodname))
            {
                Verbose(() => string.Format("<<{0}()", _methodname));
            }

            Trace.Unindent();
        }

        #endregion
    }
}