namespace Velo
{
    public interface IUnique<out T>
    {
        T Id { get; }
    }
}