using System;

namespace Velo.Dependencies.Scan
{
    internal sealed class ParentScanner : IDependencyScanner
    {
        private readonly Type _contract;

        public ParentScanner(Type contract)
        {
            _contract = contract;
        }

        public bool Applicable(Type type)
        {
            return _contract.IsAssignableFrom(type);
        }

        public void Register(DependencyBuilder builder, Type implementation)
        {
            builder.AddSingleton(_contract, implementation);
        }
    }
}