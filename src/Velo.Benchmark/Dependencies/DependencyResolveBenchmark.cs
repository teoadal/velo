using Autofac;
using BenchmarkDotNet.Attributes;
using Castle.Windsor;
using LightInject;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using Unity;
using Velo.Dependencies;
using Velo.TestsModels.Services;

namespace Velo.Benchmark.Dependencies
{
    [CoreJob]
    [MeanColumn, MemoryDiagnoser]
    public class DependencyResolveBenchmark
    {
        private IContainer _autofacContainer;
        private IWindsorContainer _castleContainer;
        private ServiceProvider _coreContainer;
        private ServiceContainer _lightInjectContainer;
        private Container _simpleContainer;
        private DependencyContainer _veloContainer;
        private IUnityContainer _unityContainer;

        [GlobalSetup]
        public void Init()
        {
            _autofacContainer = DependencyBuilders.ForAutofac().Build();
            _castleContainer = DependencyBuilders.ForCastle();
            _coreContainer = DependencyBuilders.ForCore().BuildServiceProvider();
            _lightInjectContainer = DependencyBuilders.ForLightInject();
            _simpleContainer = DependencyBuilders.ForSimpleInject();
            _veloContainer = DependencyBuilders.ForVelo().BuildContainer();
            _unityContainer = DependencyBuilders.ForUnity();
        }

        [Benchmark]
        public string Autofac()
        {
            var controller = _autofacContainer.Resolve<SomethingController>();
            var dataService = _autofacContainer.Resolve<IDataService>();
            var userService = _autofacContainer.Resolve<IUserService>();
            return controller.Name + dataService.Name + userService.Name;
        }

        [Benchmark]
        public string Castle()
        {
            var controller = _castleContainer.Resolve<SomethingController>();
            var dataService = _castleContainer.Resolve<IDataService>();
            var userService = _castleContainer.Resolve<IUserService>();
            return controller.Name + dataService.Name + userService.Name;
        }

        [Benchmark(Baseline = true)]
        public string Core()
        {
            var controller = _coreContainer.GetService<SomethingController>();
            var dataService = _coreContainer.GetService<IDataService>();
            var userService = _coreContainer.GetService<IUserService>();
            return controller.Name + dataService.Name + userService.Name;
        }

        [Benchmark]
        public string LightInject()
        {
            var controller = _lightInjectContainer.GetInstance<SomethingController>();
            var dataService = _lightInjectContainer.GetInstance<IDataService>();
            var userService = _lightInjectContainer.GetInstance<IUserService>();
            return controller.Name + dataService.Name + userService.Name;
        }
        
        [Benchmark]
        public string SimpleInject()
        {
            var controller = _simpleContainer.GetInstance<SomethingController>();
            var dataService = _simpleContainer.GetInstance<IDataService>();
            var userService = _simpleContainer.GetInstance<IUserService>();
            return controller.Name + dataService.Name + userService.Name;
        }
        
        [Benchmark]
        public string Velo()
        {
            var controller = _veloContainer.Resolve<SomethingController>();
            var dataService = _veloContainer.Resolve<IDataService>();
            var userService = _veloContainer.Resolve<IUserService>();
            return controller.Name + dataService.Name + userService.Name;
        }

        [Benchmark]
        public string Unity()
        {
            var controller = _unityContainer.Resolve<SomethingController>();
            var dataService = _unityContainer.Resolve<IDataService>();
            var userService = _unityContainer.Resolve<IUserService>();
            return controller.Name + dataService.Name + userService.Name;
        }
    }
}