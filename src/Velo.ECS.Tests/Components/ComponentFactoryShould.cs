using FluentAssertions;
using Moq;
using Velo.ECS.Components;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.ECS.Tests.Components
{
    public class ComponentFactoryShould : ECSTestClass
    {
        private readonly IComponentFactory _componentFactory;
        private readonly Mock<IComponentBuilder<TestComponent2>> _componentBuilder;

        public ComponentFactoryShould(ITestOutputHelper output) : base(output)
        {
            _componentBuilder = new Mock<IComponentBuilder<TestComponent2>>();
            _componentFactory = new ComponentFactory(new IComponentBuilder[] {_componentBuilder.Object});
        }

        [Fact]
        public void CreateComponent()
        {
            var component = _componentFactory.Create<TestComponent1>();
            component.Should().NotBeNull();
        }

        [Fact]
        public void CreateComponentWithBuilder()
        {
            _componentFactory.Create<TestComponent2>();
            _componentBuilder.Verify(builder => builder.Build());
        }
    }
}