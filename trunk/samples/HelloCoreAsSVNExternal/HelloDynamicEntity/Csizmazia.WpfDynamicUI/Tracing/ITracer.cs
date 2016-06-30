using System;

namespace Csizmazia.Tracing
{
    public interface ITracer
    {
        void Verbose(Func<string> message);
        void Info(Func<string> message);
        void Warn(Func<string> message);

        void Error(Func<string> message);
        void Error(Func<string> message, Exception ex);


        void Critical(Func<string> message, Exception ex);
    }
}