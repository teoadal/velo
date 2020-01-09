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
                if (cancellationToken.IsCancellationRequested) break;
                await initSystem.Initialize(cancellationToken);
            }
        }

        public async Task Update(CancellationToken cancellationToken)
        {
            var beforeUpdateSystems = _provider.GetServices<IBeforeUpdateSystem>();
            foreach (var beforeUpdateSystem in beforeUpdateSystems)
            {
                if (cancellationToken.IsCancellationRequested) break;
                await beforeUpdateSystem.BeforeUpdate(cancellationToken);
            }

            var updateSystems = _provider.GetServices<IUpdateSystem>();
            foreach (var updateSystem in updateSystems)
            {
                if (cancellationToken.IsCancellationRequested) break;
                await updateSystem.Update(cancellationToken);
            }

            var afterUpdateSystems = _provider.GetServices<IAfterUpdateSystem>();
            foreach (var afterUpdateSystem in afterUpdateSystems)
            {
                if (cancellationToken.IsCancellationRequested) break;
                await afterUpdateSystem.AfterUpdate(cancellationToken);
            }
        }
    }
}