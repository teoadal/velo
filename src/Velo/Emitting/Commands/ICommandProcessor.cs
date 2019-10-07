using System;

namespace Velo.Emitting.Commands
{
    internal interface ICommandProcessor
    {
        bool Applicable(Type type);
    }
    
    internal interface ICommandProcessor<in TCommand> : ICommandProcessor
        where TCommand : ICommand
    {
        void Execute(TCommand command);
    }
}