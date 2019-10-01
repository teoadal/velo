using System;
using Velo.CQRS.Commands;
using Velo.CQRS.Queries;
using Velo.Dependencies;

namespace Velo.CQRS
{
    public static class BusDependencies
    {
        public static DependencyBuilder AddCommandHandler<THandler>(this DependencyBuilder builder)
            where THandler : ICommandHandler
        {
            return builder.AddSingleton<ICommandHandler, THandler>();
        }

        public static DependencyBuilder AddCommandHandler<TCommand>(this DependencyBuilder builder,
            Action<HandlerContext, TCommand> handler)
            where TCommand : ICommand
        {
            return builder.AddSingleton<ICommandHandler>(ctx => new AnonymousCommandHandler<TCommand>(ctx, handler));
        }

        public static DependencyBuilder AddQueryHandler<THandler>(this DependencyBuilder builder)
            where THandler : IQueryHandler
        {
            return builder.AddSingleton<IQueryHandler, THandler>();
        }

        public static DependencyBuilder AddQueryHandler<TQuery, TResult>(this DependencyBuilder builder,
            Func<HandlerContext, TQuery, TResult> handler)
            where TQuery : IQuery<TResult>
        {
            return builder.AddSingleton<IQueryHandler>(ctx => new AnonymousQueryHandler<TQuery, TResult>(ctx, handler));
        }

        public static DependencyBuilder UseBus(this DependencyBuilder builder)
        {
            return builder.AddSingleton(ctx => new Bus(ctx));
        }
    }
}