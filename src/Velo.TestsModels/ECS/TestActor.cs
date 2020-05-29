using System;
using Velo.ECS.Actors;
using Velo.ECS.Assets;
using Velo.ECS.Components;
using Velo.ECS.Sources.Json;

namespace Velo.TestsModels.ECS
{
    public sealed class TestActor : Actor
    {
        [SerializeReference] 
        public Asset Prototype { get; set; }

        public TestActor(int id, IComponent[] components = null)
            : base(id, components ?? Array.Empty<IComponent>())
        {
        }
    }
}