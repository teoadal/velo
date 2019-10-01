using System;
using System.Collections.Generic;
using System.Diagnostics;
using AutoFixture.Xunit2;
using Velo.CQRS;
using Velo.Dependencies;
using Velo.Serialization;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Velo
{
    public class CqrsTests : IDisposable
    {
        private readonly Bus _bus;
        private readonly DependencyContainer _container;
        private readonly ITestOutputHelper _output;
        private readonly Stopwatch _stopwatch;

        public CqrsTests(ITestOutputHelper output)
        {
            _container = new DependencyBuilder()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddCommandHandler<CreateBooHandler>()
                .AddQueryHandler<GetBooHandler>()
                .UseBus()
                .BuildContainer();

            _bus = _container.Resolve<Bus>();

            _output = output;
            _stopwatch = Stopwatch.StartNew();
        }

        [Theory, AutoData]
        public void Ask(int id)
        {
            var repository = _container.Resolve<IBooRepository>();
            repository.AddElement(new Boo {Id = id});

            var boo = _bus.Ask(new GetBoo {Id = id});

            Assert.Equal(id, boo.Id);
        }

        [Theory, AutoData]
        public void Ask_Anonymous(int id)
        {
            var container = new DependencyBuilder()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddQueryHandler<GetBoo, Boo>((ctx, payload) => ctx
                    .Resolve<IBooRepository>()
                    .GetElement(payload.Id))
                .UseBus()
                .BuildContainer();

            var bus = new Bus(container);

            var repository = container.Resolve<IBooRepository>();
            repository.AddElement(new Boo {Id = id});

            var boo = bus.Ask(new GetBoo {Id = id});

            Assert.Equal(id, boo.Id);
        }

        [Theory, AutoData]
        public void Ask_Concrete(int id)
        {
            var repository = _container.Resolve<IBooRepository>();
            repository.AddElement(new Boo {Id = id});

            var boo = _bus.Ask<GetBoo, Boo>(new GetBoo {Id = id});

            Assert.Equal(id, boo.Id);
        }
        
        [Theory, AutoData]
        public void Execute(int id, bool boolean, int number)
        {
            _bus.Execute(new CreateBoo {Id = id, Bool = boolean, Int = number});

            var repository = _container.Resolve<IBooRepository>();
            var boo = repository.GetElement(id);

            Assert.Equal(id, boo.Id);
            Assert.Equal(boolean, boo.Bool);
            Assert.Equal(number, boo.Int);
        }

        [Theory, AutoData]
        public void Execute_Anonymous(int id, bool boolean, int number)
        {
            var container = new DependencyBuilder()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddCommandHandler<CreateBoo>((ctx, payload) => ctx
                    .Resolve<IBooRepository>()
                    .AddElement(new Boo {Id = payload.Id, Bool = payload.Bool, Int = payload.Int}))
                .UseBus()
                .BuildContainer();

            var bus = new Bus(container);

            bus.Execute(new CreateBoo {Id = id, Bool = boolean, Int = number});

            var repository = container.Resolve<IBooRepository>();
            var boo = repository.GetElement(id);

            Assert.Equal(id, boo.Id);
            Assert.Equal(boolean, boo.Bool);
            Assert.Equal(number, boo.Int);
        }

        [Fact]
        public void Throw_CommandHandler_Not_Registered()
        {
            var bus = new Bus(new DependencyBuilder().BuildContainer());

            Assert.Throws<KeyNotFoundException>(() => bus.Execute(new CreateBoo()));
        }

        [Fact]
        public void Throw_QueryHandler_Not_Registered()
        {
            var bus = new Bus(new DependencyBuilder().BuildContainer());

            Assert.Throws<KeyNotFoundException>(() => bus.Ask(new GetBoo()));
        }

        public void Dispose()
        {
            _output.WriteLine($"Elapsed {_stopwatch.ElapsedMilliseconds} ms");
        }
    }
}