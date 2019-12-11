using System.Runtime.CompilerServices;
using Velo.Ordering;

namespace Velo.ECS.Systems
{
    internal sealed class SystemService
    {
        private readonly IInitializeSystem[] _initializeSystems;
        private readonly IBeforeUpdateSystem[] _beforeUpdateSystems;
        private readonly IUpdateSystem[] _updateSystems;
        private readonly IAfterUpdateSystem[] _afterUpdateSystems;

        public SystemService(IInitializeSystem[] initializeSystems, IBeforeUpdateSystem[] beginUpdateSystems,
            IAfterUpdateSystem[] endUpdateSystems, IUpdateSystem[] updateSystems)
        {
            _initializeSystems = SortByOrderAttribute(initializeSystems);
            _beforeUpdateSystems = SortByOrderAttribute(beginUpdateSystems);
            _afterUpdateSystems = SortByOrderAttribute(endUpdateSystems);
            _updateSystems = SortByOrderAttribute(updateSystems);
        }

        public void Initialize()
        {
            foreach (var initSystem in _initializeSystems)
            {
                initSystem.Initialize();
            }
        }

        public void Update()
        {
            foreach (var beforeUpdateSystem in _beforeUpdateSystems)
            {
                beforeUpdateSystem.BeforeUpdate();
            }

            foreach (var updateSystem in _updateSystems)
            {
                updateSystem.Update();
            }

            foreach (var afterUpdateSystem in _afterUpdateSystems)
            {
                afterUpdateSystem.AfterUpdate();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T[] SortByOrderAttribute<T>(T[] array) where T : class
        {
            return OrderAttributeComparer<T>.Sort(array);
        }
    }
}