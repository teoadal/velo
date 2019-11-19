using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands
{
    internal sealed class CommandExecutor<TCommand> : ICommandExecutor
        where TCommand : ICommand
    {
        private readonly Type _preProcessorsType;
        private readonly Type _processorType;
        private readonly Type _postProcessorsType;

        public CommandExecutor()
        {
            _preProcessorsType = typeof(ICommandPreProcessor<TCommand>[]);
            _processorType = typeof(ICommandProcessor<TCommand>);
            _postProcessorsType = typeof(ICommandPostProcessor<TCommand>[]);
        }

        public async Task Execute(IServiceProvider provider, TCommand command, CancellationToken cancellationToken)
        {
            var preProcessors = (ICommandPreProcessor<TCommand>[]) provider.GetService(_preProcessorsType);
            foreach (var preProcessor in preProcessors)
            {
                if (cancellationToken.IsCancellationRequested) return;
                await preProcessor.PreProcess(command, cancellationToken);
            }

            var processor = (ICommandProcessor<TCommand>) provider.GetService(_processorType);
            await processor.Process(command, cancellationToken);

            var postProcessors = (ICommandPostProcessor<TCommand>[]) provider.GetService(_postProcessorsType);
            foreach (var postProcessor in postProcessors)
            {
                if (cancellationToken.IsCancellationRequested) return;
                await postProcessor.PostProcess(command, cancellationToken);
            }
        }
    }

    internal interface ICommandExecutor
    {
    }
}