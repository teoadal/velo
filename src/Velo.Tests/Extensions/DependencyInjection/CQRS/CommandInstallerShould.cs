using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Velo.CQRS.Commands;
using Velo.TestsModels.Emitting.Boos.Create;
using Xunit;

namespace Velo.Tests.Extensions.DependencyInjection.CQRS
{
    public class CommandInstallerShould : ServiceCollectionTests
    {
        private readonly IServiceCollection _services;

        public CommandInstallerShould()
        {
            _services = new ServiceCollection()
                .AddEmitter();
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void AddBehaviour(ServiceLifetime lifetime)
        {
            _services.AddCommandBehaviour<Behaviour>(lifetime);

            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(ICommandBehaviour<Command>) &&
                descriptor.ImplementationType == typeof(Behaviour) &&
                descriptor.Lifetime == lifetime);
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void AddPreProcessor(ServiceLifetime lifetime)
        {
            _services.AddCommandProcessor<PreProcessor>(lifetime);

            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(ICommandPreProcessor<Command>) &&
                descriptor.ImplementationType == typeof(PreProcessor) &&
                descriptor.Lifetime == lifetime);
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void AddProcessor(ServiceLifetime lifetime)
        {
            _services.AddCommandProcessor<Processor>(lifetime);

            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(ICommandProcessor<Command>) &&
                descriptor.ImplementationType == typeof(Processor) &&
                descriptor.Lifetime == lifetime);
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void AddPostProcessor(ServiceLifetime lifetime)
        {
            _services.AddCommandProcessor<PostProcessor>(lifetime);

            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(ICommandPostProcessor<Command>) &&
                descriptor.ImplementationType == typeof(PostProcessor) &&
                descriptor.Lifetime == lifetime);
        }
    }
}