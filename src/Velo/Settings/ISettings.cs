namespace Velo.Settings
{
    public interface ISettings<out T>
    {
        T Get();
    }
}