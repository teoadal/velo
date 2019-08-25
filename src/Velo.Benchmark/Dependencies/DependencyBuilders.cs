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
using Velo.TestsModels;
using Velo.TestsModels.Services;

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

            builder.RegisterType<DataService>().As<IDataService>().SingleInstance();
            builder.RegisterType<DataRepository>().As<IDataRepository>().SingleInstance();

            builder.RegisterType<UserService>().As<IUserService>().SingleInstance();
            builder.RegisterType<UserRepository>().As<IUserRepository>().SingleInstance();

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
                
                Component.For<IDataService>().ImplementedBy<DataService>().LifeStyle.Singleton,
                Component.For<IDataRepository>().ImplementedBy<DataRepository>().LifeStyle.Singleton,
                
                Component.For<IUserService>().ImplementedBy<UserService>().LifeStyle.Singleton,
                Component.For<IUserRepository>().ImplementedBy<UserRepository>().LifeStyle.Singleton,
                
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
                
                .AddSingleton<IDataService, DataService>()
                .AddSingleton<IDataRepository, DataRepository>()
                
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<IUserRepository, UserRepository>()
                
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
                
                .RegisterSingleton<IDataService, DataService>()
                .RegisterSingleton<IDataRepository, DataRepository>()
                
                .RegisterSingleton<IUserService, UserService>()
                .RegisterSingleton<IUserRepository, UserRepository>()
                
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

            container.RegisterSingleton<IDataService, DataService>();
            container.RegisterSingleton<IDataRepository, DataRepository>();

            container.RegisterSingleton<IUserService, UserService>();
            container.RegisterSingleton<IUserRepository, UserRepository>();
                
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
                .AddSingleton<IConfiguration>(provider => new Configuration())
                .AddFactory<ISession, Session>()
                
                .AddSingleton<IDataService, DataService>()
                .AddSingleton<IDataRepository, DataRepository>()
                
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<IUserRepository, UserRepository>()
                
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
                
                .RegisterSingleton<IDataService, DataService>()
                .RegisterSingleton<IDataRepository, DataRepository>()
                
                .RegisterSingleton<IUserService, UserService>()
                .RegisterSingleton<IUserRepository, UserRepository>()
                
                .RegisterSingleton<SomethingController>();
        }
    }
}