using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Velo.CQRS;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.PingPong;

namespace Velo.Benchmark.CQRS
{
    public static class MediatorBuilder
    {
        public static IMediator BuildMediatR(IBooRepository repository,
            Action<IServiceCollection> dependencyBuilder = null, int repeatDependencyBuilder = 1)
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton(repository)
                .AddSingleton<IRequestHandler<GetBooRequest, Boo>, GetBooHandler>()
                .AddSingleton<IRequestHandler<StructRequest, StructResponse>, StructRequestHandler>()
                .AddScoped<IMediator>(scope => new Mediator(scope.GetService));

            if (dependencyBuilder != null)
            {
                for (var i = 0; i < repeatDependencyBuilder; i++)
                {
                    dependencyBuilder(serviceCollection);        
                }
            }

            return serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();
        }

        public static IEmitter BuildEmitter(IBooRepository repository,
            Action<IServiceCollection> dependencyBuilder = null, int repeatDependencyBuilder = 1)
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton(repository)
                .AddQueryProcessor<PingPongProcessor>()
                .AddQueryProcessor<TestsModels.Emitting.Boos.Get.Processor>()
                .AddEmitter();

            if (dependencyBuilder != null)
            {
                for (var i = 0; i < repeatDependencyBuilder; i++)
                {
                    dependencyBuilder(serviceCollection);        
                }
            }
            
            return serviceCollection
                .BuildServiceProvider()
                .GetService<IEmitter>();
        }
    }
}