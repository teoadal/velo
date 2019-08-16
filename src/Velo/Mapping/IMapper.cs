namespace Velo.Mapping
{
    public interface IMapper<out TOut>
    {
        TOut Map(object source);
    }
}