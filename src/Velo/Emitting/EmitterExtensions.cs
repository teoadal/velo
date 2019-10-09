using System;
using Velo.Dependencies;
using Velo.Emitting.Commands;
using Velo.Emitting.Queries;

namespace Velo.Emitting
{
    public static class EmitterExtensions
    {
        private static readonly Type CommandHandlerType = typeof(ICommandHandler);
        private static readonly Type QueryHandlerType = typeof(IQueryHandler);
        
        public static DependencyBuilder AddCommandHandler<THandler>(this DependencyBuilder builder)
            where THandler : ICommandHandler
        {
            var implementation = typeof(THandler);
            return builder.AddSingleton(new [] { implementation, CommandHandlerType }, implementation);
        }

        public static DependencyBuilder AddCommandHandler<TCommand>(this DependencyBuilder builder,
            Action<EmitterContext, TCommand> handler)
            where TCommand : ICommand
        {
            return builder.AddSingleton<ICommandHandler>(ctx => new AnonymousCommandHandler<TCommand>(ctx, handler));
        }

        public static DependencyBuilder AddQueryHandler<THandler>(this DependencyBuilder builder)
            where THandler : IQueryHandler
        {
            var implementation = typeof(THandler);
            return builder.AddSingleton(new [] { implementation, QueryHandlerType }, implementation);
        }

        public static DependencyBuilder AddQueryHandler<TQuery, TResult>(this DependencyBuilder builder,
            Func<EmitterContext, TQuery, TResult> handler)
            where TQuery : IQuery<TResult>
        {
            return builder.AddSingleton<IQueryHandler>(ctx => new AnonymousQueryHandler<TQuery, TResult>(ctx, handler));
        }

        public static DependencyBuilder AddEmitter(this DependencyBuilder builder)
        {
            return builder.AddSingleton<Emitter>();
        }
    }
}