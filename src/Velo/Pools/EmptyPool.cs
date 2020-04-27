namespace Velo.Pools
{
    internal sealed class EmptyPool<T>: IPool<T>
        where T: class
    {
        private readonly T _defaultValue;
        
        public EmptyPool(T defaultValue)
        {
            _defaultValue = defaultValue;
        }
        
        public T Get()
        {
            return _defaultValue;
        }

        public bool Return(T element)
        {
            return true;
        }

        public bool TryGet(out T element)
        {
            element = _defaultValue;
            return true;
        }
    }
}