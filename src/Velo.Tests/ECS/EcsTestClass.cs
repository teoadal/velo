using System;
using Velo.ECS;
using Velo.ECS.Actors;
using Velo.ECS.Assets;
using Velo.TestsModels.ECS;
using Xunit.Abstractions;

namespace Velo.Tests.ECS
{
    public abstract class EcsTestClass : TestClass
    {
        public EcsTestClass(ITestOutputHelper output) : base(output)
        {
        }
        
        protected static Actor BuildActor(int id)
        {
            return new Actor(id, BuildComponents());
        }

        protected static Actor BuildActor<TComponent>(int id)
            where TComponent: IComponent
        {
            return new Actor(id, new IComponent[] { Activator.CreateInstance<TComponent>() });
        }
        
        protected static Asset BuildAsset(int id)
        {
            return new Asset(id, BuildComponents());
        }

        protected static Asset BuildAsset<TComponent>(int id)
            where TComponent: IComponent
        {
            return new Asset(id, new IComponent[] { Activator.CreateInstance<TComponent>() });
        }
        
        protected static IComponent[] BuildComponent<TComponent>()
            where TComponent: IComponent, new()
        {
            return new IComponent[]
            {
                new TComponent(), 
            };
        }
        
        protected static IComponent[] BuildComponents()
        {
            return new IComponent[]
            {
                new HealthComponent(), new ManaCostComponent()
            };
        }
    }
}