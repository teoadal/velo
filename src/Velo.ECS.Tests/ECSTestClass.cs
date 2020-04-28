using AutoFixture;
using Moq;
using Velo.DependencyInjection;
using Velo.ECS.Actors;
using Velo.ECS.Actors.Context;
using Velo.ECS.Components;
using Velo.TestsModels;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.ECS.Tests
{
    // ReSharper disable once InconsistentNaming
    public abstract class ECSTestClass : TestClass
    {
        protected ECSTestClass(ITestOutputHelper output) : base(output)
        {
        }

        protected void InjectComponentsArray(IComponent[]? array = null)
        {
            array ??= new IComponent[]
            {
                new TestComponent1(),
                new TestComponent2()
            };

            Fixture.Inject(array);
        }
        
        protected void RaiseActorAdded(Mock<IActorContext> context, Actor actor)
        {
            context.Raise(ctx => ctx.Added += null, actor);
        }

        protected void RaiseActorRemoved(Mock<IActorContext> context, Actor actor)
        {
            context.Raise(ctx => ctx.Removed += null, actor);
        }
        
        public static TheoryData<DependencyLifetime> Lifetimes => new TheoryData<DependencyLifetime>
        {
            DependencyLifetime.Scoped,
            DependencyLifetime.Singleton,
            DependencyLifetime.Transient
        };

    }
}