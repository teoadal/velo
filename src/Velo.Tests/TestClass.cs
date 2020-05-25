using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Velo.Collections;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.Serialization;

namespace Velo.Tests
{
    public abstract class TestClass : IDisposable
    {
        protected static readonly CancellationToken CancellationToken = CancellationToken.None;

        protected Fixture Fixture => _fixture ??= new Fixture();

        private Fixture _fixture;

        internal static ConvertersCollection BuildConvertersCollection(IServiceProvider serviceProvider = null)
        {
            serviceProvider ??= new DependencyCollection()
                .AddJsonConverter()
                .BuildProvider();

            return (ConvertersCollection) serviceProvider.GetService(typeof(IConvertersCollection));
        }

        protected static Mock<IDependency> MockDependency(
            DependencyLifetime lifetime = DependencyLifetime.Singleton,
            Type contract = null)
        {
            var dependency = new Mock<IDependency>();
            dependency
                .SetupGet(d => d.Lifetime)
                .Returns(lifetime);

            // ReSharper disable once InvertIf
            if (contract != null)
            {
                dependency
                    .Setup(d => d.Applicable(contract))
                    .Returns(true);

                dependency
                    .SetupGet(d => d.Contracts)
                    .Returns(new[] {contract});

                dependency
                    .SetupGet(d => d.Implementation)
                    .Returns(contract);
            }

            return dependency;
        }

        protected static Mock<IDependency> MockDependency<T>(
            DependencyLifetime lifetime = DependencyLifetime.Singleton,
            IServiceProvider serviceProvider = null) where T : class
        {
            var contract = typeof(T);

            var dependency = MockDependency(lifetime, contract);

            if (serviceProvider != null)
            {
                dependency
                    .Setup(d => d.GetInstance(contract, serviceProvider))
                    .Returns(Mock.Of<T>());
            }

            return dependency;
        }

        protected static void SetupApplicable(Mock<IDependencyEngine> engine, Type contract,
            params IDependency[] result)
        {
            engine
                .Setup(e => e.GetApplicable(contract))
                .Returns(result);
        }

        protected static int EnsureValid(int count, int maxValue = 10000)
        {
            count = Math.Abs(count);
            if (count > maxValue) count = maxValue;

            return count;
        }

        protected static T[] Many<T>(Func<T> action)
        {
            var result = new T[5];
            for (var i = 0; i < 5; i++)
            {
                result[i] = action();
            }

            return result;
        }

        protected static T[] Many<T>(int count, Func<int, T> action)
        {
            var result = new T[count];
            for (var i = 0; i < count; i++)
            {
                result[i] = action(i);
            }

            return result;
        }

        protected static object MockOf(Type contract)
        {
            return MockType(contract).Object;
        }

        protected static Mock MockType(Type contract)
        {
            var mockObjectType = typeof(Mock<>).MakeGenericType(contract);
            return (Mock) Activator.CreateInstance(mockObjectType);
        }

        protected static Task RunTasks(int count, Action action)
        {
            var tasks = new Task[count];
            for (var i = 0; i < count; i++)
            {
                tasks[i] = Task.Run(action);
            }

            return Task.WhenAll(tasks);
        }

        protected static Mock<IDependency>[] SetupApplicableDependencies(
            Mock<IDependencyEngine> dependencyEngine,
            Type type,
            DependencyLifetime lifetime = DependencyLifetime.Singleton,
            int count = 10)
        {
            var dependencies = Enumerable
                .Range(0, count)
                .Select(_ => new Mock<IDependency>())
                .Do(dependency => dependency
                    .SetupGet(d => d.Lifetime)
                    .Returns(lifetime))
                .Do(dependency => dependency
                    .SetupGet(d => d.Implementation)
                    .Returns(type))
                .ToArray();

            dependencyEngine
                .Setup(engine => engine.Contains(type))
                .Returns(true);

            dependencyEngine
                .Setup(engine => engine.GetApplicable(type))
                .Returns(dependencies.Select(d => d.Object).ToArray());

            return dependencies;
        }

        protected static void SetupRequiredDependency(
            Mock<IDependencyEngine> dependencyEngine,
            Type type,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            dependencyEngine.Setup(engine => engine.Contains(type)).Returns(true);
            dependencyEngine
                .Setup(engine => engine.GetRequiredDependency(type))
                .Returns(Mock.Of<IDependency>(d => d.Lifetime == lifetime));
        }

        protected static void SetupRequiredDependencies(
            Mock<IDependencyEngine> dependencyEngine,
            Type type,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            dependencyEngine
                .Setup(engine => engine.Contains(type))
                .Returns(true);

            dependencyEngine
                .Setup(engine => engine.GetRequiredDependency(type.MakeArrayType()))
                .Returns(Mock.Of<IDependency>(d => d.Lifetime == lifetime));
        }

        public virtual void Dispose()
        {
        }
    }
}