using Autofac;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Castle.Windsor;
using LightInject;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using Unity;
using Velo.DependencyInjection;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;

namespace Velo.Benchmark.DependencyInjection
{
    [SimpleJob(RuntimeMoniker.NetCoreApp22)]
    [MeanColumn, MemoryDiagnoser]
    [CategoriesColumn, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class DependencyResolveBenchmark
    {
        private IContainer _autofacContainer;
        private IWindsorContainer _castleContainer;
        private ServiceProvider _coreContainer;
        private ServiceProvider _coreContainerMixed;
        private ServiceContainer _lightInjectContainer;
        private Container _simpleContainer;
        private DependencyProvider _veloContainer;
        private DependencyProvider _veloContainerMixed;
        private IUnityContainer _unityContainer;

        [GlobalSetup]
        public void Init()
        {
//            _autofacContainer = DependencyBuilders.ForAutofac().Build();
//            _castleContainer = DependencyBuilders.ForCastle();
            _coreContainer = DependencyBuilders.ForCore().BuildServiceProvider();
            _coreContainerMixed = DependencyBuilders.ForCore_Mixed().BuildServiceProvider();
//            _lightInjectContainer = DependencyBuilders.ForLightInject();
//            _simpleContainer = DependencyBuilders.ForSimpleInject();
            _veloContainer = DependencyBuilders.ForVelo().BuildProvider();
            _veloContainerMixed = DependencyBuilders.ForVelo_Mixed().BuildProvider();
//            _unityContainer = DependencyBuilders.ForUnity();
        }

//        [Benchmark]
//        public string Autofac()
//        {
//            var controller = _autofacContainer.Resolve<SomethingController>();
//            var dataService = _autofacContainer.Resolve<IFooService>();
//            var userService = _autofacContainer.Resolve<IBooService>();
//            return controller.Name + dataService.Name + userService.Name;
//        }
//
//        [Benchmark]
//        public string Castle()
//        {
//            var controller = _castleContainer.Resolve<SomethingController>();
//            var dataService = _castleContainer.Resolve<IFooService>();
//            var userService = _castleContainer.Resolve<IBooService>();
//            return controller.Name + dataService.Name + userService.Name;
//        }

        [BenchmarkCategory("Singleton"), Benchmark(Baseline = true)]
        public string Core()
        {
            var controller = _coreContainer.GetService<SomethingController>();
            var dataService = _coreContainer.GetService<IFooService>();
            var userService = _coreContainer.GetService<IBooService>();
            return controller.Name + dataService.Name + userService.Name;
        }

        [BenchmarkCategory("Mixed"), Benchmark(Baseline = true, OperationsPerInvoke = 10)]
        public string Core_Mixed()
        {
            using (var scope = _coreContainerMixed.CreateScope())
            {
                var controller = scope.ServiceProvider.GetService<SomethingController>();
                var dataService = scope.ServiceProvider.GetService<IFooService>();
                var userService = scope.ServiceProvider.GetService<IBooService>();

                return controller.Name + dataService.Name + userService.Name;
            }
        }

//        [Benchmark]
//        public string LightInject()
//        {
//            var controller = _lightInjectContainer.GetInstance<SomethingController>();
//            var dataService = _lightInjectContainer.GetInstance<IFooService>();
//            var userService = _lightInjectContainer.GetInstance<IBooService>();
//            return controller.Name + dataService.Name + userService.Name;
//        }
//        
//        [Benchmark]
//        public string SimpleInject()
//        {
//            var controller = _simpleContainer.GetInstance<SomethingController>();
//            var dataService = _simpleContainer.GetInstance<IFooService>();
//            var userService = _simpleContainer.GetInstance<IBooService>();
//            return controller.Name + dataService.Name + userService.Name;
//        }

        [BenchmarkCategory("Singleton"), Benchmark]
        public string Velo()
        {
            var controller = _veloContainer.GetService<SomethingController>();
            var dataService = _veloContainer.GetService<IFooService>();
            var userService = _veloContainer.GetService<IBooService>();

            return controller.Name + dataService.Name + userService.Name;
        }

        [BenchmarkCategory("Mixed"), Benchmark(OperationsPerInvoke = 10)]
        public string Velo_Mixed()
        {
            using (var scope = _veloContainerMixed.CreateScope())
            {
                var controller = scope.GetService<SomethingController>();
                var dataService = scope.GetService<IFooService>();
                var userService = scope.GetService<IBooService>();

                return controller.Name + dataService.Name + userService.Name;
            }
        }

//        [Benchmark]
//        public string Unity()
//        {
//            var controller = _unityContainer.Resolve<SomethingController>();
//            var dataService = _unityContainer.Resolve<IFooService>();
//            var userService = _unityContainer.Resolve<IBooService>();
//            return controller.Name + dataService.Name + userService.Name;
//        }
    }
}