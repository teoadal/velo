using System;
using System.Collections.Concurrent;
using Velo.Utils;

namespace Velo.CQRS.Commands
{
    internal sealed class CommandRouter : IDisposable
    {
        public static readonly Type[] ProcessorTypes = {
            typeof(ICommandPreProcessor<>),
            typeof(ICommandProcessor<>), 
            typeof(ICommandPostProcessor<>)
        };

        private static readonly Type ExecutorType = typeof(CommandExecutor<>);

        private Func<Type, ICommandExecutor> _buildExecutor;
        private ConcurrentDictionary<Type, ICommandExecutor> _executors;

        public CommandRouter()
        {
            _buildExecutor = BuildExecutor;
            _executors = new ConcurrentDictionary<Type, ICommandExecutor>();
        }

        public CommandExecutor<TCommand> GetExecutor<TCommand>() where TCommand : ICommand
        {
            var commandType = Typeof<TCommand>.Raw;
            var executor = _executors.GetOrAdd(commandType, _buildExecutor);

            return (CommandExecutor<TCommand>) executor;
        }

        private static ICommandExecutor BuildExecutor(Type commandType)
        {
            var executorType = ExecutorType.MakeGenericType(commandType);
            return (ICommandExecutor) Activator.CreateInstance(executorType);
        }

        public void Dispose()
        {
            _buildExecutor = null!;
            
            _executors.Clear();
            _executors = null!;
        }
    }
}