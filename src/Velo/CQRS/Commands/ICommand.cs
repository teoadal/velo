namespace Velo.CQRS.Commands
{
    public interface ICommand
    {
        bool StopPropagation { get; }
    }
}