namespace Velo.Pools
{
    public interface IArrayPool<T>
    {
        T[] Get(int length);

        IPool<T[]> GetPool(int length);
        
        void Return(T[] array,  bool clearArray = false);

        bool TryGet(int length, out T[] array);
    }
}