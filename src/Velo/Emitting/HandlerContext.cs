namespace Velo.Emitting
{
    public sealed class HandlerContext<T>
    {
        public bool StopPropagation { get; set; }

        public T Payload { get; set; }
        
        internal HandlerContext(T payload)
        {
            Payload = payload;
        }
    }
}