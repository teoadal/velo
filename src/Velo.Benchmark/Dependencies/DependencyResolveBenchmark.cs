using Autofac;
using BenchmarkDotNet.Attributes;
using Castle.Windsor;
using LightInject;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using Unity;
using Velo.Dependencies;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;

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
            var dataService = _autofacContainer.Resolve<IFooService>();
            var userService = _autofacContainer.Resolve<IBooService>();
            return controller.Name + dataService.Name + userService.Name;
        }

        [Benchmark]
        public string Castle()
        {
            var controller = _castleContainer.Resolve<SomethingController>();
            var dataService = _castleContainer.Resolve<IFooService>();
            var userService = _castleContainer.Resolve<IBooService>();
            return controller.Name + dataService.Name + userService.Name;
        }

        [Benchmark(Baseline = true)]
        public string Core()
        {
            var controller = _coreContainer.GetService<SomethingController>();
            var dataService = _coreContainer.GetService<IFooService>();
            var userService = _coreContainer.GetService<IBooService>();
            return controller.Name + dataService.Name + userService.Name;
        }

        [Benchmark]
        public string LightInject()
        {
            var controller = _lightInjectContainer.GetInstance<SomethingController>();
            var dataService = _lightInjectContainer.GetInstance<IFooService>();
            var userService = _lightInjectContainer.GetInstance<IBooService>();
            return controller.Name + dataService.Name + userService.Name;
        }
        
        [Benchmark]
        public string SimpleInject()
        {
            var controller = _simpleContainer.GetInstance<SomethingController>();
            var dataService = _simpleContainer.GetInstance<IFooService>();
            var userService = _simpleContainer.GetInstance<IBooService>();
            return controller.Name + dataService.Name + userService.Name;
        }
        
        [Benchmark]
        public string Velo()
        {
            var controller = _veloContainer.Resolve<SomethingController>();
            var dataService = _veloContainer.Resolve<IFooService>();
            var userService = _veloContainer.Resolve<IBooService>();
            return controller.Name + dataService.Name + userService.Name;
        }

        [Benchmark]
        public string Unity()
        {
            var controller = _unityContainer.Resolve<SomethingController>();
            var dataService = _unityContainer.Resolve<IFooService>();
            var userService = _unityContainer.Resolve<IBooService>();
            return controller.Name + dataService.Name + userService.Name;
        }
    }
}