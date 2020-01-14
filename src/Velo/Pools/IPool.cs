namespace Velo.Pools
{
    public interface IPool<T> where T : class
    {
        T Get();
        
        bool Return(T element);

        bool TryGet(out T element);
    }
}