using Moq;
using Velo.Tests.NewECS.Actors;
using Velo.Tests.NewECS.Actors.Context;
using Xunit.Abstractions;

namespace Velo.Tests.NewECS.Tests
{
    // ReSharper disable once InconsistentNaming
    public abstract class ECSTestClass : TestClass
    {
        protected ECSTestClass(ITestOutputHelper output) : base(output)
        {
        }

        protected void RaiseActorAdded(Mock<IActorContext> context, Actor actor)
        {
            context.Raise(ctx => ctx.Added += null, actor);
        }

        protected void RaiseActorRemoved(Mock<IActorContext> context, Actor actor)
        {
            context.Raise(ctx => ctx.Removed += null, actor);
        }
    }
}