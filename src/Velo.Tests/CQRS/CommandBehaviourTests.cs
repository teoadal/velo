using System;
using System.Threading.Tasks;
using Velo.DependencyInjection;
using Velo.Serialization;
using Velo.Settings;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;
using Command = Velo.TestsModels.Emitting.Boos.Create.Command;
using Processor = Velo.TestsModels.Emitting.Boos.Create.Processor;

namespace Velo.CQRS
{
    public class CommandPipelineTests : TestClass
    {
        private DependencyCollection _dependencyCollection;

        public CommandPipelineTests(ITestOutputHelper output) : base(output)
        {
            // 9.9% --> 2 млн (5 лет) 42 000
            // 
            _dependencyCollection = new DependencyCollection()
                .AddSingleton<IConfiguration>(ctx => new Configuration())
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddEmitter();
        }

        [Fact]
        public async Task CallBehaviour()
        {
            var provider = _dependencyCollection
                .AddCommandProcessor<Processor>()
                .AddCommandBehaviour<MeasureBehaviour<Command>>()
                .BuildProvider();

            var mediator = provider.GetRequiredService<Emitter>();

            await mediator.Execute(new Command {Id = 1});

            var measureBehaviour = provider.GetRequiredService<MeasureBehaviour<Command>>();
            Assert.True(measureBehaviour.Elapsed > TimeSpan.Zero);
        }

        [Fact]
        public async Task CatchException()
        {
            var provider = _dependencyCollection
                .AddCommandProcessor<Processor>()
                .AddCommandBehaviour<MeasureBehaviour<Command>>()
                .AddCommandBehaviour<ExceptionBehaviour<Command>>()
                .BuildProvider();

            var mediator = provider.GetRequiredService<Emitter>();

            await mediator.Execute(new Command {Id = -1});

            var measureBehaviour = provider.GetRequiredService<MeasureBehaviour<Command>>();
            Assert.True(measureBehaviour.Elapsed > TimeSpan.Zero);
            
            var exceptionBehaviour = provider.GetRequiredService<ExceptionBehaviour<Command>>();
            Assert.NotNull(exceptionBehaviour.Exception);
            Assert.IsType<InvalidOperationException>(exceptionBehaviour.Exception);
        }
    }
}