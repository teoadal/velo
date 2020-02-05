using System;
using System.Diagnostics;

namespace Velo.DependencyInjection.Resolvers
{
    [DebuggerDisplay("Implementation = {_instance.GetType().Name}")]
    internal sealed class InstanceResolver : DependencyResolver
    {
        private readonly object _instance;

        public InstanceResolver(object instance): base(instance.GetType())
        {
            _instance = instance;
        }

        protected override object GetInstance(Type contract, IDependencyScope scope)
        {
            return _instance;
        }
    }
}