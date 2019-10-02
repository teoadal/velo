using System;
using System.Diagnostics;
using AutoFixture.Xunit2;
using Velo.Dependencies;
using Velo.Serialization;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Boos.Emitting;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Emitting
{
    public class AnonymousEmitHandlers : IDisposable
    {
        private readonly DependencyBuilder _builder;
        private readonly ITestOutputHelper _output;
        private readonly Stopwatch _stopwatch;

        public AnonymousEmitHandlers(ITestOutputHelper output)
        {
            _builder = new DependencyBuilder()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddEmitter();

            _output = output;
            _stopwatch = Stopwatch.StartNew();
        }

        [Theory, AutoData]
        public void Ask_Anonymous(int id)
        {
            var container = _builder
                .AddQueryHandler<GetBoo, Boo>((ctx, payload) => ctx
                    .Resolve<IBooRepository>()
                    .GetElement(payload.Id))
                .BuildContainer();

            var emitter = new Emitter(container);

            var repository = container.Resolve<IBooRepository>();
            repository.AddElement(new Boo {Id = id});

            var boo = emitter.Ask(new GetBoo {Id = id});

            Assert.Equal(id, boo.Id);
        }

        [Theory, AutoData]
        public void Execute_Anonymous(int id, bool boolean, int number)
        {
            var container = _builder
                .AddCommandHandler<CreateBoo>((ctx, payload) => ctx
                    .Resolve<IBooRepository>()
                    .AddElement(new Boo {Id = payload.Id, Bool = payload.Bool, Int = payload.Int}))
                .BuildContainer();

            var emitter = container.Resolve<Emitter>();

            emitter.Execute(new CreateBoo {Id = id, Bool = boolean, Int = number});

            var repository = container.Resolve<IBooRepository>();
            var boo = repository.GetElement(id);

            Assert.Equal(id, boo.Id);
            Assert.Equal(boolean, boo.Bool);
            Assert.Equal(number, boo.Int);
        }

        public void Dispose()
        {
            _output.WriteLine($"Elapsed {_stopwatch.ElapsedMilliseconds} ms");
        }
    }
}