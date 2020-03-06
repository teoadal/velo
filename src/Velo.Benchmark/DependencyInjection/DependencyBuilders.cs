using Autofac;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using LightInject;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using Unity;
using Unity.Lifetime;
using Velo.DependencyInjection;
using Velo.Logging;
using Velo.Logging.Provider;
using Velo.Mapping;
using Velo.Serialization;
using Velo.Settings;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;
using NullProvider = Velo.Logging.Provider.NullProvider;

namespace Velo.Benchmark.DependencyInjection
{
    public static class DependencyBuilders
    {
        public static ContainerBuilder ForAutofac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<JConverter>().SingleInstance();
            builder.RegisterType<Logger<SomethingController>>().As<ILogger<SomethingController>>().SingleInstance();
            builder.RegisterType<NullProvider>().As<ILogProvider>().SingleInstance();
            builder.RegisterType<CompiledMapper<Boo>>().As<IMapper<Boo>>().SingleInstance();
            builder.RegisterType<CompiledMapper<Foo>>().As<IMapper<Foo>>().SingleInstance();
            builder.Register(ctx => new Settings.Provider.NullProvider()).As<ISettings>().SingleInstance();
            builder.RegisterType<Session>().As<ISession>().SingleInstance();

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
                Component.For<ILogger<SomethingController>>().ImplementedBy<Logger<SomethingController>>().LifeStyle.Singleton,
                Component.For<ILogProvider>().ImplementedBy<NullProvider>().LifeStyle.Singleton,
                Component.For<IMapper<Boo>>().ImplementedBy<CompiledMapper<Boo>>().LifeStyle.Singleton,
                Component.For<IMapper<Foo>>().ImplementedBy<CompiledMapper<Foo>>().LifeStyle.Singleton,
                Component.For<ISettings>().UsingFactoryMethod(() => new Settings.Provider.NullProvider()).LifeStyle.Singleton, 
                Component.For<ISession>().ImplementedBy<Session>().LifeStyle.Singleton,
                
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
                .AddSingleton<ILogger<SomethingController>, Logger<SomethingController>>()
                .AddSingleton<ILogProvider, NullProvider>()
                .AddSingleton<IMapper<Boo>, CompiledMapper<Boo>>()
                .AddSingleton<IMapper<Foo>, CompiledMapper<Foo>>()
                .AddSingleton<ISettings>(ctx => new Settings.Provider.NullProvider())
                .AddSingleton<ISession, Session>()
                
                .AddSingleton<IFooService, FooService>()
                .AddSingleton<IFooRepository, FooRepository>()
                
                .AddSingleton<IBooService, BooService>()
                .AddSingleton<IBooRepository, BooRepository>()
                
                .AddSingleton<SomethingController>();
        }

        public static IServiceCollection ForCore_Mixed()
        {
            return new ServiceCollection()
                .AddSingleton<JConverter>()
                .AddSingleton<ILogger<SomethingController>, Logger<SomethingController>>()
                .AddSingleton<ILogProvider, NullProvider>()
                .AddSingleton<IMapper<Boo>, CompiledMapper<Boo>>()
                .AddSingleton<IMapper<Foo>, CompiledMapper<Foo>>()
                .AddSingleton<ISettings>(ctx => new Settings.Provider.NullProvider())
                .AddTransient<ISession, Session>()
                
                .AddScoped<IFooService, FooService>()
                .AddScoped<IFooRepository, FooRepository>()
                
                .AddScoped<IBooService, BooService>()
                .AddScoped<IBooRepository, BooRepository>()
                
                .AddScoped<SomethingController>();
        }
        
        public static ServiceContainer ForLightInject()
        {
            var container = new ServiceContainer();
            
            container
                .RegisterSingleton(provider => new JConverter())
                .RegisterSingleton<ILogger<SomethingController>, Logger<SomethingController>>()
                .RegisterSingleton<ILogProvider, NullProvider>()
                .RegisterSingleton<IMapper<Boo>, CompiledMapper<Boo>>()
                .RegisterSingleton<IMapper<Foo>, CompiledMapper<Foo>>()
                .RegisterSingleton<ISettings>(provider => new Settings.Provider.NullProvider())
                .RegisterSingleton<ISession, Session>()
                
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
            container.RegisterSingleton<ILogger<SomethingController>, Logger<SomethingController>>();
            container.RegisterSingleton<ILogProvider, NullProvider>();
            container.RegisterSingleton<IMapper<Boo>, CompiledMapper<Boo>>();
            container.RegisterSingleton<IMapper<Foo>, CompiledMapper<Foo>>();
            container.Register(typeof(ISettings), () => new Settings.Provider.NullProvider(), Lifestyle.Singleton);
            container.RegisterSingleton<ISession, Session>();

            container.RegisterSingleton<IFooService, FooService>();
            container.RegisterSingleton<IFooRepository, FooRepository>();

            container.RegisterSingleton<IBooService, BooService>();
            container.RegisterSingleton<IBooRepository, BooRepository>();
                
            container.RegisterSingleton<SomethingController>();
            
            return container;
        }
        
        public static DependencyCollection ForVelo()
        {
            return new DependencyCollection()
                .AddSingleton<JConverter>()
                .AddSingleton<ILogger<SomethingController>, Logger<SomethingController>>()
                .AddSingleton<ILogProvider, NullProvider>()
                .AddSingleton<IMapper<Boo>, CompiledMapper<Boo>>()
                .AddSingleton<IMapper<Foo>, CompiledMapper<Foo>>()
                .AddSingleton<ISettings>(ctx => new Settings.Provider.NullProvider())
                .AddSingleton<ISession, Session>()
                
                .AddSingleton<IFooService, FooService>()
                .AddSingleton<IFooRepository, FooRepository>()
                
                .AddSingleton<IBooService, BooService>()
                .AddSingleton<IBooRepository, BooRepository>()
                
                .AddSingleton<SomethingController>();
        }
        
        public static DependencyCollection ForVelo_Mixed()
        {
            return new DependencyCollection()
                .AddSingleton<JConverter>()
                .AddSingleton<ILogger<SomethingController>, Logger<SomethingController>>()
                .AddSingleton<ILogProvider, NullProvider>()
                .AddSingleton<IMapper<Boo>, CompiledMapper<Boo>>()
                .AddSingleton<IMapper<Foo>, CompiledMapper<Foo>>()
                .AddSingleton<ISettings>(ctx => new Settings.Provider.NullProvider())
                .AddTransient<ISession, Session>()
                
                .AddScoped<IFooService, FooService>()
                .AddScoped<IFooRepository, FooRepository>()
                
                .AddScoped<IBooService, BooService>()
                .AddScoped<IBooRepository, BooRepository>()
                
                .AddScoped<SomethingController>();
        }
        
        public static IUnityContainer ForUnity()
        {
            return new UnityContainer()
                .RegisterSingleton<JConverter>()
                .RegisterSingleton<ILogger<SomethingController>, Logger<SomethingController>>()
                .RegisterSingleton<ILogProvider, NullProvider>()
                .RegisterSingleton<IMapper<Boo>, CompiledMapper<Boo>>()
                .RegisterSingleton<IMapper<Foo>, CompiledMapper<Foo>>()
                .RegisterFactory<ISettings>(ctx => new Settings.Provider.NullProvider(), new SingletonLifetimeManager())
                .RegisterSingleton<ISession, Session>()
                
                .RegisterSingleton<IFooService, FooService>()
                .RegisterSingleton<IFooRepository, FooRepository>()
                
                .RegisterSingleton<IBooService, BooService>()
                .RegisterSingleton<IBooRepository, BooRepository>()
                
                .RegisterSingleton<SomethingController>();
        }
    }
}