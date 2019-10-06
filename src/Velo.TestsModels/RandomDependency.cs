using System;
using Velo.Dependencies;

namespace Velo.TestsModels
{
    public class RandomDependency : IDependency
    {
        private readonly Type _contract;
        private readonly int _seed;

        public RandomDependency()
        {
            _contract = typeof(Random);
            _seed = DateTime.Now.Second;
        }

        public bool Applicable(Type contract)
        {
            return _contract == contract;
        }

        public void Init(DependencyContainer container)
        {
        }

        public void Destroy()
        {
        }

        public object Resolve(Type contract, DependencyContainer container)
        {
            return new Random(_seed);
        }
    }
}