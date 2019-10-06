namespace Velo.TestsModels.Domain
{
    public class Manager<T> : IManager<T>
    {
        public bool Disposed { get; private set; }

        public void Do(T data)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            Disposed = true;
        }
    }
}