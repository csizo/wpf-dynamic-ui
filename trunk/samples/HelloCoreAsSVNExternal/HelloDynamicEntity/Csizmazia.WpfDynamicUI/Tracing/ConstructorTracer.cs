namespace Csizmazia.Tracing
{
    public class ConstructorTracer<T> : MethodTracer<T>
    {
        public ConstructorTracer()
            : base(typeof (T).Name + ".ctor")
        {
        }
    }
}