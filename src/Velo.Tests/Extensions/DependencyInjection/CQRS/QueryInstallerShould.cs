using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Velo.CQRS.Queries;
using Velo.CQRS.Queries.Pipeline;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Get;
using Xunit;
using Xunit.Abstractions;
using Behaviour = Velo.TestsModels.Emitting.Boos.Get.Behaviour;
using PostProcessor = Velo.TestsModels.Emitting.Boos.Get.PostProcessor;
using PreProcessor = Velo.TestsModels.Emitting.Boos.Get.PreProcessor;
using Processor = Velo.TestsModels.Emitting.Boos.Get.Processor;

namespace Velo.Tests.Extensions.DependencyInjection.CQRS
{
    public class QueryInstallerShould : ServiceCollectionTests
    {
        private readonly IServiceCollection _services;

        public QueryInstallerShould(ITestOutputHelper output) : base(output)
        {
            _services = new ServiceCollection()
                .AddEmitter();
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void AddBehaviour(ServiceLifetime lifetime)
        {
            _services.AddQueryBehaviour<Behaviour>(lifetime);

            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(IQueryBehaviour<Query, Boo>) &&
                descriptor.ImplementationType == typeof(Behaviour) &&
                descriptor.Lifetime == lifetime);
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void AddPreProcessor(ServiceLifetime lifetime)
        {
            _services.AddQueryProcessor<PreProcessor>(lifetime);

            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(IQueryPreProcessor<Query, Boo>) &&
                descriptor.ImplementationType == typeof(PreProcessor) &&
                descriptor.Lifetime == lifetime);
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void AddProcessor(ServiceLifetime lifetime)
        {
            _services.AddQueryProcessor<Processor>(lifetime);

            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(IQueryProcessor<Query, Boo>) &&
                descriptor.ImplementationType == typeof(Processor) &&
                descriptor.Lifetime == lifetime);
        }

        [Fact]
        public void AddProcessorInstance()
        {
            var instance = BuildProcessor();
            _services.AddQueryProcessor(instance);

            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(IQueryProcessor<Query, Boo>) &&
                descriptor.ImplementationInstance.IsSameOrEqualTo(instance) &&
                descriptor.Lifetime == ServiceLifetime.Singleton);
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void AddPostProcessor(ServiceLifetime lifetime)
        {
            _services.AddQueryProcessor<PostProcessor>(lifetime);

            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(IQueryPostProcessor<Query, Boo>) &&
                descriptor.ImplementationType == typeof(PostProcessor) &&
                descriptor.Lifetime == lifetime);
        }

        [Fact]
        public void ResolveFullPipeline()
        {
            _services
                .AddQueryBehaviour(BuildBehaviour())
                .AddQueryProcessor(BuildPreProcessor())
                .AddQueryProcessor(BuildProcessor())
                .AddQueryProcessor(BuildPostProcessor())
                .BuildServiceProvider()
                .GetRequiredService<IQueryPipeline<Query, Boo>>()
                .Should().BeOfType<QueryFullPipeline<Query, Boo>>();
        }

        [Fact]
        public void ResolveSimplePipeline()
        {
            _services
                .AddQueryProcessor(BuildProcessor())
                .BuildServiceProvider()
                .GetRequiredService<IQueryPipeline<Query, Boo>>()
                .Should().BeOfType<QuerySimplePipeline<Query, Boo>>();
        }

        [Fact]
        public void ResolveSequentialPipeline()
        {
            _services
                .AddQueryProcessor(BuildPreProcessor())
                .AddQueryProcessor(BuildProcessor())
                .AddQueryProcessor(BuildPostProcessor())
                .BuildServiceProvider()
                .GetRequiredService<IQueryPipeline<Query, Boo>>()
                .Should().BeOfType<QuerySequentialPipeline<Query, Boo>>();
        }

        private static IQueryBehaviour<Query, Boo> BuildBehaviour()
        {
            return Mock.Of<IQueryBehaviour<Query, Boo>>();
        }

        private static IQueryProcessor<Query, Boo> BuildProcessor()
        {
            return Mock.Of<IQueryProcessor<Query, Boo>>();
        }

        private static IQueryPostProcessor<Query, Boo> BuildPostProcessor()
        {
            return Mock.Of<IQueryPostProcessor<Query, Boo>>();
        }

        private static IQueryPreProcessor<Query, Boo> BuildPreProcessor()
        {
            return Mock.Of<IQueryPreProcessor<Query, Boo>>();
        }
    }
}