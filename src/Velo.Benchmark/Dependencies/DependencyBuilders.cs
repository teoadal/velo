using Autofac;

using Castle.MicroKernel.Registration;
using Castle.Windsor;

using Microsoft.Extensions.DependencyInjection;

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

            builder.RegisterType<DataUserController>().SingleInstance();

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
                
                Component.For<DataUserController>().LifeStyle.Singleton
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
                
                .AddSingleton<DataUserController>();
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
                
                .AddSingleton<DataUserController>();
        }
    }
}