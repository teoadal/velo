using Autofac;

using Castle.MicroKernel.Registration;
using Castle.Windsor;
using LightInject;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using Unity;
using Unity.Lifetime;
using Velo.Dependencies;
using Velo.Mapping;
using Velo.Serialization;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;

namespace Velo.Benchmark.Dependencies
{
    public static class DependencyBuilders
    {
        public static ContainerBuilder ForAutofac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<JConverter>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();
            builder.RegisterType<CompiledMapper<Boo>>().As<IMapper<Boo>>().SingleInstance();
            builder.RegisterType<CompiledMapper<Foo>>().As<IMapper<Foo>>().SingleInstance();
            builder.Register(ctx => new Configuration()).As<IConfiguration>().SingleInstance();
            builder.RegisterType<Session>().As<ISession>();

            builder.RegisterType<FooService>().As<IFooService>().SingleInstance();
            builder.RegisterType<FooRepository>().As<IFooRepository>().SingleInstance();

            builder.RegisterType<BooService>().As<IBooService>().SingleInstance();
            builder.RegisterType<BooRepository>().As<IBooRepository>().SingleInstance();

            builder.RegisterType<SomethingController>().SingleInstance();

            return builder;
        }

        public static IWindsorContainer ForCastle()
        {
            return new WindsorContainer().Register(
                Component.For<JConverter>().LifeStyle.Singleton,
                Component.For<ILogger>().ImplementedBy<Logger>().LifeStyle.Singleton,
                Component.For<IMapper<Boo>>().ImplementedBy<CompiledMapper<Boo>>().LifeStyle.Singleton,
                Component.For<IMapper<Foo>>().ImplementedBy<CompiledMapper<Foo>>().LifeStyle.Singleton,
                Component.For<IConfiguration>().UsingFactoryMethod(() => new Configuration()).LifeStyle.Singleton, 
                Component.For<ISession>().ImplementedBy<Session>().LifeStyle.Transient,
                
                Component.For<IFooService>().ImplementedBy<FooService>().LifeStyle.Singleton,
                Component.For<IFooRepository>().ImplementedBy<FooRepository>().LifeStyle.Singleton,
                
                Component.For<IBooService>().ImplementedBy<BooService>().LifeStyle.Singleton,
                Component.For<IBooRepository>().ImplementedBy<BooRepository>().LifeStyle.Singleton,
                
                Component.For<SomethingController>().LifeStyle.Singleton
            );
        }

        public static IServiceCollection ForCore()
        {
            return new ServiceCollection()
                .AddSingleton<JConverter>()
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<IMapper<Boo>, CompiledMapper<Boo>>()
                .AddSingleton<IMapper<Foo>, CompiledMapper<Foo>>()
                .AddSingleton<IConfiguration>(provider => new Configuration())
                .AddTransient<ISession, Session>()
                
                .AddSingleton<IFooService, FooService>()
                .AddSingleton<IFooRepository, FooRepository>()
                
                .AddSingleton<IBooService, BooService>()
                .AddSingleton<IBooRepository, BooRepository>()
                
                .AddSingleton<SomethingController>();
        }

        public static ServiceContainer ForLightInject()
        {
            var container = new ServiceContainer();
            
            container
                .RegisterSingleton(provider => new JConverter())
                .RegisterSingleton<ILogger, Logger>()
                .RegisterSingleton<IMapper<Boo>, CompiledMapper<Boo>>()
                .RegisterSingleton<IMapper<Foo>, CompiledMapper<Foo>>()
                .RegisterSingleton<IConfiguration>(provider => new Configuration())
                .RegisterTransient<ISession, Session>()
                
                .RegisterSingleton<IFooService, FooService>()
                .RegisterSingleton<IFooRepository, FooRepository>()
                
                .RegisterSingleton<IBooService, BooService>()
                .RegisterSingleton<IBooRepository, BooRepository>()
                
                .RegisterSingleton<SomethingController>();

            return container;
        }

        public static Container ForSimpleInject()
        {
            var container = new Container();
            
            container.Register(typeof(JConverter), () => new JConverter(), Lifestyle.Singleton);
            container.RegisterSingleton<ILogger, Logger>();
            container.RegisterSingleton<IMapper<Boo>, CompiledMapper<Boo>>();
            container.RegisterSingleton<IMapper<Foo>, CompiledMapper<Foo>>();
            container.Register(typeof(IConfiguration), () => new Configuration(), Lifestyle.Singleton);
            container.RegisterSingleton<ISession, Session>();

            container.RegisterSingleton<IFooService, FooService>();
            container.RegisterSingleton<IFooRepository, FooRepository>();

            container.RegisterSingleton<IBooService, BooService>();
            container.RegisterSingleton<IBooRepository, BooRepository>();
                
            container.RegisterSingleton<SomethingController>();
            
            return container;
        }
        
        public static DependencyBuilder ForVelo()
        {
            return new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<IMapper<Boo>, CompiledMapper<Boo>>()
                .AddSingleton<IMapper<Foo>, CompiledMapper<Foo>>()
                .AddSingleton<IConfiguration>(ctx => new Configuration())
                .AddTransient<ISession, Session>()
                
                .AddSingleton<IFooService, FooService>()
                .AddSingleton<IFooRepository, FooRepository>()
                
                .AddSingleton<IBooService, BooService>()
                .AddSingleton<IBooRepository, BooRepository>()
                
                .AddSingleton<SomethingController>();
        }

        public static IUnityContainer ForUnity()
        {
            return new UnityContainer()
                .RegisterSingleton<JConverter>()
                .RegisterSingleton<ILogger, Logger>()
                .RegisterSingleton<IMapper<Boo>, CompiledMapper<Boo>>()
                .RegisterSingleton<IMapper<Foo>, CompiledMapper<Foo>>()
                .RegisterFactory<IConfiguration>(ctx => new Configuration(), new SingletonLifetimeManager())
                .RegisterFactory<ISession>(ctx => new Session(ctx.Resolve<JConverter>()))
                
                .RegisterSingleton<IFooService, FooService>()
                .RegisterSingleton<IFooRepository, FooRepository>()
                
                .RegisterSingleton<IBooService, BooService>()
                .RegisterSingleton<IBooRepository, BooRepository>()
                
                .RegisterSingleton<SomethingController>();
        }
    }
}