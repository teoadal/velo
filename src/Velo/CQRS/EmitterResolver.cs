using Velo.CQRS.Commands;
using Velo.CQRS.Queries;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Engine;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.CQRS
{
    internal sealed class EmitterResolver : DependencyResolver
    {
        private CommandRouter _commandRouter;
        private QueryRouter _queryRouter;

        public EmitterResolver() : base(Typeof<Emitter>.Raw, DependencyLifetime.Scope)
        {
        }

        public override object Resolve(DependencyProvider scope)
        {
            return new Emitter(scope, _commandRouter, _queryRouter);
        }

        protected override void Initialize(DependencyEngine engine)
        {
            _commandRouter = new CommandRouter();
            _queryRouter = new QueryRouter();
        }
    }
}