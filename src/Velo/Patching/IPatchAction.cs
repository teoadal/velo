namespace Velo.Patching
{
    public interface IPatchAction<in T> where T : class
    {
        void Apply(T instance);
    }
}