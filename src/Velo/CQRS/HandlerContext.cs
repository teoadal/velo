using System.Collections.Generic;
using Velo.Dependencies;
using Velo.Utils;

namespace Velo.CQRS
{
    public class HandlerContext
    {
        private readonly DependencyContainer _container;
        private readonly Dictionary<int, IDependency> _dependencies;

        internal HandlerContext(DependencyContainer container)
        {
            _container = container;
            _dependencies = new Dictionary<int, IDependency>(0);
        }

        public T Resolve<T>()
        {
            var type = Typeof<T>.Raw;
            var typeId = Typeof<T>.Id;

            if (_dependencies.TryGetValue(typeId, out var existsDependency))
            {
                return (T) existsDependency.Resolve(type, _container);
            }
            
            var dependency = _container.GetDependency(type);
            _dependencies.Add(typeId, dependency);

            return (T) dependency.Resolve(type, _container);
        }
    }
}