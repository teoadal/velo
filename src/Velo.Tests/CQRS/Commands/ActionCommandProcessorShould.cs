using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.CQRS;
using Velo.DependencyInjection;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Create;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS.Commands
{
    public class ActionCommandProcessorShould : TestClass
    {
        private readonly DependencyCollection _dependencyCollection;

        public ActionCommandProcessorShould(ITestOutputHelper output) : base(output)
        {
            _dependencyCollection = new DependencyCollection()
                .AddEmitter();
        }

        [Theory]
        [AutoData]
        public void Executed(Command command)
        {
            var processor = new Mock<Action<Command>>();

            var emitter = _dependencyCollection
                .CreateProcessor(processor.Object)
                .BuildProvider()
                .GetRequiredService<IEmitter>();

            emitter.Awaiting(e => e.Execute(command)).Should().NotThrow();

            processor.Verify(p => p.Invoke(command));
        }

        [Theory]
        [AutoData]
        public void ExecutedWithContext(Command command)
        {
            var repository = Mock.Of<IBooRepository>();
            var processor = new Mock<Action<Command, IBooRepository>>();

            var emitter = _dependencyCollection
                .AddInstance(repository)
                .CreateProcessor(processor.Object)
                .BuildProvider()
                .GetRequiredService<IEmitter>();

            emitter.Awaiting(e => e.Execute(command)).Should().NotThrow();

            processor.Verify(p => p.Invoke(command, repository));
        }
    }
}