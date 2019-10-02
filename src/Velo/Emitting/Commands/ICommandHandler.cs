namespace Velo.Emitting.Commands
{
    public interface ICommandHandler<TCommand> : ICommandHandler
        where TCommand : ICommand
    {
        void Execute(HandlerContext<TCommand> context);
    }

    public interface ICommandHandler
    {
    }
}