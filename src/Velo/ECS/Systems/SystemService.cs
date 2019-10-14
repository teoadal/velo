using System.Runtime.CompilerServices;
using Velo.Ordering;

namespace Velo.ECS.Systems
{
    internal sealed class SystemService
    {
        private readonly IInitializeSystem[] _initializeSystems;
        private readonly IBeginUpdateSystem[] _beginUpdateSystems;
        private readonly IUpdateSystem[] _updateSystems;
        private readonly IEndUpdateSystem[] _endUpdateSystems;

        public SystemService(IInitializeSystem[] initializeSystems, IBeginUpdateSystem[] beginUpdateSystems,
            IEndUpdateSystem[] endUpdateSystems, IUpdateSystem[] updateSystems)
        {
            _initializeSystems = SortByOrderAttribute(initializeSystems);
            _beginUpdateSystems = SortByOrderAttribute(beginUpdateSystems);
            _endUpdateSystems = SortByOrderAttribute(endUpdateSystems);
            _updateSystems = SortByOrderAttribute(updateSystems);
        }

        public void Initialize()
        {
            for (var i = 0; i < _initializeSystems.Length; i++)
            {
                _initializeSystems[i].Initialize();
            }
        }

        public void Update()
        {
            for (var i = 0; i < _beginUpdateSystems.Length; i++)
            {
                _beginUpdateSystems[i].BeginUpdate();
            }

            for (var i = 0; i < _initializeSystems.Length; i++)
            {
                _updateSystems[i].Update();
            }

            for (var i = 0; i < _endUpdateSystems.Length; i++)
            {
                _endUpdateSystems[i].EndUpdate();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T[] SortByOrderAttribute<T>(T[] array) where T : class
        {
            return OrderAttributeComparer<T>.Sort(array);
        }
    }
}