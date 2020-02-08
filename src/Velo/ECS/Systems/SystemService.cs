using System.Threading;
using System.Threading.Tasks;
using Velo.DependencyInjection;

namespace Velo.ECS.Systems
{
    internal sealed class SystemService
    {
        private readonly DependencyProvider _provider;

        public SystemService(DependencyProvider provider)
        {
            _provider = provider;
        }

        public async Task Initialize(CancellationToken cancellationToken)
        {
            var initSystems = _provider.GetServices<IInitializeSystem>();
            foreach (var initSystem in initSystems)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await initSystem.Initialize(cancellationToken);
            }
        }

        public async Task Update(CancellationToken cancellationToken)
        {
            var beforeUpdateSystems = _provider.GetServices<IBeforeUpdateSystem>();
            foreach (var beforeUpdateSystem in beforeUpdateSystems)
            {
                await beforeUpdateSystem.BeforeUpdate(cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();
            
            var updateSystems = _provider.GetServices<IUpdateSystem>();
            foreach (var updateSystem in updateSystems)
            {
                await updateSystem.Update(cancellationToken);
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            
            var afterUpdateSystems = _provider.GetServices<IAfterUpdateSystem>();
            foreach (var afterUpdateSystem in afterUpdateSystems)
            {
                await afterUpdateSystem.AfterUpdate(cancellationToken);
            }
        }
    }
}