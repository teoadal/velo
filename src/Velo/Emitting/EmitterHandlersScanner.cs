using System;
using Velo.Dependencies;
using Velo.Dependencies.Scan;
using Velo.Emitting.Commands;
using Velo.Emitting.Queries;

namespace Velo.Emitting
{
    internal sealed class EmitterHandlersScanner : IDependencyScanner
    {
        private readonly Type _commandHandlerType;
        private readonly Type _queryHandlerType;

        public EmitterHandlersScanner()
        {
            _commandHandlerType = typeof(ICommandHandler);
            _queryHandlerType = typeof(IQueryHandler);
        }
        
        public bool Applicable(Type type)
        {
            return _commandHandlerType.IsAssignableFrom(type)
                   || _queryHandlerType.IsAssignableFrom(type);
        }

        public void Register(DependencyBuilder builder, Type implementation)
        {
            builder.AddSingleton(_commandHandlerType.IsAssignableFrom(implementation)
                ? new[] {_commandHandlerType, implementation}
                : new[] {_queryHandlerType, implementation}, implementation);
        }

    }
}