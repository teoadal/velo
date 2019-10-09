namespace Velo.Emitting.Commands
{
    public interface ICommand
    {
        bool StopPropagation { get; }
    }
}