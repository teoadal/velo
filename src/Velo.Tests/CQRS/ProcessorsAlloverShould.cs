using FluentAssertions;
using Velo.DependencyInjection;
using Velo.TestsModels;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS
{
    public class ProcessorsAlloverShould : TestClass
    {
        private readonly DependencyCollection _dependencies;

        public ProcessorsAlloverShould(ITestOutputHelper output) : base(output)
        {
            _dependencies = new DependencyCollection()
                .Scan(scanner => scanner
                    .AssemblyOf<TestsModels.Emitting.Boos.Create.Processor>()
                    .AddEmitterProcessors());
        }

        [Fact]
        public void FindBehaviours()
        {
            _dependencies.Contains(typeof(TestsModels.Emitting.Boos.Get.Behaviour)).Should().BeTrue();
            _dependencies.Contains(typeof(TestsModels.Emitting.MeasureBehaviour)).Should().BeTrue();
        }

        [Fact]
        public void FindPreProcessors()
        {
            _dependencies.Contains(typeof(TestsModels.Emitting.Boos.Create.PreProcessor)).Should().BeTrue();
            _dependencies.Contains(typeof(TestsModels.Emitting.Boos.Get.PreProcessor)).Should().BeTrue();
            _dependencies.Contains(typeof(TestsModels.Emitting.Boos.Create.NotificationProcessor)).Should().BeTrue();
        }

        [Fact]
        public void FindProcessors()
        {
            _dependencies.Contains(typeof(TestsModels.Emitting.Boos.Create.Processor)).Should().BeTrue();
            _dependencies.Contains(typeof(TestsModels.Emitting.Boos.Get.Processor)).Should().BeTrue();
            _dependencies.Contains(typeof(TestsModels.Emitting.Boos.Update.Processor)).Should().BeTrue();
            _dependencies.Contains(typeof(TestsModels.Emitting.Foos.Create.Processor)).Should().BeTrue();
            _dependencies.Contains(typeof(TestsModels.Emitting.Foos.Create.OnBooCreated)).Should().BeTrue();
        }

        [Fact]
        public void FindPostProcessors()
        {
            _dependencies.Contains(typeof(TestsModels.Emitting.Boos.Create.PostProcessor)).Should().BeTrue();
            _dependencies.Contains(typeof(TestsModels.Emitting.Boos.Get.PostProcessor)).Should().BeTrue();
        }
    }
}