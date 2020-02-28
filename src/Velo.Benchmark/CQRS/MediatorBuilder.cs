using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Velo.CQRS;
using Velo.Extensions.DependencyInjection.CQRS;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.PingPong;
using Boos = Velo.TestsModels.Emitting.Boos;

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
                .AddScoped<IMediator>(ctx => new Mediator(ctx.GetService));

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
                .AddQueryProcessor<Boos.Get.Processor>()
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
                .GetService<Emitter>();
        }
    }
}