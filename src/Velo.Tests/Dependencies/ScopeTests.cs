using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Velo.Mapping;
using Velo.Serialization;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Dependencies
{
    public class ScopeTests : TestBase
    {
        private readonly DependencyBuilder _builder;

        public ScopeTests(ITestOutputHelper output) : base(output)
        {
            _builder = new DependencyBuilder()
                .AddScope<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IConfiguration, Configuration>()
                .AddGenericSingleton(typeof(IMapper<>), typeof(CompiledMapper<>));
        }

        [Fact]
        public void Scope()
        {
            var container = _builder.BuildContainer();

            ISession firstScopeSession;
            using (container.StartScope())
            {
                firstScopeSession = container.Resolve<ISession>();
                Assert.Same(firstScopeSession, container.Resolve<ISession>());
            }

            using (container.StartScope())
            {
                var secondScopeSession = container.Resolve<ISession>();
                Assert.NotSame(firstScopeSession, secondScopeSession);
            }
        }

        [Fact]
        public void Scope_Circular_Dependency()
        {
            var container = _builder
                .AddScope<CircularDependencyService>()
                .BuildContainer();

            using (container.StartScope())
            {
                Assert.Throws<TypeAccessException>(() => container.Resolve<CircularDependencyService>());
            }
        }

        [Fact]
        public void Scope_Destroy_After_End()
        {
            var container = _builder
                .AddSingleton<IFooRepository, FooRepository>()
                .AddScope<IFooService, FooService>()
                .BuildContainer();

            IFooService service;
            using (container.StartScope())
            {
                service = container.Resolve<IFooService>();
                Assert.False(service.Disposed);
            }

            Assert.True(service.Disposed);
        }

        [Fact]
        public void Scope_Generic()
        {
            var container = _builder
                .AddGenericScope(typeof(List<>))
                .BuildContainer();

            List<int> firstScopeList;
            using (container.StartScope())
            {
                firstScopeList = container.Resolve<List<int>>();
                Assert.Same(firstScopeList, container.Resolve<List<int>>());
            }

            using (container.StartScope())
            {
                var secondScopeList = container.Resolve<List<int>>();
                Assert.NotSame(firstScopeList, secondScopeList);
            }
        }

        [Fact]
        public void Scope_Generic_With_Contract()
        {
            var container = _builder
                .AddGenericScope(typeof(IList<>), typeof(List<>))
                .BuildContainer();

            IList<int> firstScopeList;
            using (container.StartScope())
            {
                firstScopeList = container.Resolve<IList<int>>();
                Assert.Same(firstScopeList, container.Resolve<IList<int>>());
            }

            using (container.StartScope())
            {
                var secondScopeList = container.Resolve<IList<int>>();
                Assert.NotSame(firstScopeList, secondScopeList);
            }
        }

        [Fact]
        public void Scope_Many()
        {
            var container = _builder
                .AddScope<IFooRepository, FooRepository>()
                .AddScope<IFooService, FooService>()
                .BuildContainer();

            const int scopesCount = 10;

            var fooServices = new IFooService[scopesCount];
            for (var i = 0; i < fooServices.Length; i++)
            {
                using (container.StartScope())
                {
                    fooServices[i] = container.Resolve<IFooService>();
                }
            }

            for (var i = 0; i < fooServices.Length; i++)
            for (var j = 0; j < fooServices.Length; j++)
            {
                if (i == j) continue;
                Assert.NotSame(fooServices[i], fooServices[j]);
            }
        }

        [Fact]
        public void Scope_MultiThreading()
        {
            var container = _builder
                .AddScope<IFooService, FooService>()
                .AddSingleton<IFooRepository, FooRepository>()
                .BuildContainer();

            const int scopesCount = 10;

            var fooServices = new ConcurrentBag<IFooService>();
            var tasks = new Task[scopesCount];
            for (var i = 0; i < scopesCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    using (container.StartScope())
                    {
                        fooServices.Add(container.Resolve<IFooService>());
                    }
                });
            }

            Task.WaitAll(tasks);

            var services = fooServices.ToArray();
            for (var i = 0; i < services.Length; i++)
            for (var j = 0; j < services.Length; j++)
            {
                if (i == j) continue;
                Assert.NotSame(services[i], services[j]);
            }
        }

        [Fact]
        public void Scope_Nested()
        {
            var container = _builder.BuildContainer();

            using (container.StartScope())
            {
                var session = container.Resolve<ISession>();
                Assert.Same(session, container.Resolve<ISession>());

                using (container.StartScope())
                {
                    Assert.Same(session, container.Resolve<ISession>());
                }

                Assert.Same(session, container.Resolve<ISession>());
            }
        }

        [Fact]
        public void Scope_Throw_If_Resolve_WithoutScope()
        {
            var container = _builder.BuildContainer();

            Assert.Throws<InvalidOperationException>(() => container.Resolve<ISession>());
        }
    }
}