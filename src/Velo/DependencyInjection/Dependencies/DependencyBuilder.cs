using System;
using System.Collections.Generic;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection.Dependencies
{
    public struct DependencyBuilder
    {
        private List<Type> _contracts;
        private Type _implementation;
        private DependencyLifetime _lifetime;

        public DependencyBuilder Contract(Type type)
        {
            _contracts ??= new List<Type>();

            _contracts.Add(type);

            return this;
        }

        public DependencyBuilder Contract<TContract>()
        {
            _contracts ??= new List<Type>();

            _contracts.Add(Typeof<TContract>.Raw);

            return this;
        }

        public DependencyBuilder Contracts<TContract1, TContract2>()
        {
            _contracts ??= new List<Type>();

            _contracts.Add(Typeof<TContract1>.Raw);
            _contracts.Add(Typeof<TContract2>.Raw);

            return this;
        }

        public DependencyBuilder Implementation<TImplementation>()
        {
            _implementation = typeof(TImplementation);
            return this;
        }

        public DependencyBuilder Lifetime(DependencyLifetime lifetime)
        {
            _lifetime = lifetime;
            return this;
        }

        public DependencyBuilder Scoped() => Lifetime(DependencyLifetime.Scoped);

        public DependencyBuilder Singleton() => Lifetime(DependencyLifetime.Singleton);

        public DependencyBuilder Transient() => Lifetime(DependencyLifetime.Transient);

        internal IDependency Build(IDependencyEngine engine)
        {
            var resolver = DependencyResolver.Build(_lifetime, _implementation, engine);
            return Dependency.Build(_lifetime, _contracts.ToArray(), resolver);
        }
    }
}