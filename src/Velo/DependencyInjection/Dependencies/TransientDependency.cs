using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection.Dependencies
{
    internal sealed class TransientDependency : Dependency
    {
        private bool _disposed;
        
        public TransientDependency(DependencyResolver resolver) : base(resolver)
        {
        }

        public override object GetInstance(DependencyProvider scope)
        {
            if (_disposed)
            {
                throw Error.Disposed(nameof(TransientDependency));
            }
            
            return Resolver.Resolve(scope);
        }

        public override void Dispose()
        {
            if (_disposed) return;
            
            _disposed = true;

            base.Dispose();
        }
    }
}