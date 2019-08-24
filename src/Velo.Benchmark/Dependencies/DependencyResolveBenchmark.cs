using Autofac;

using BenchmarkDotNet.Attributes;

using Castle.Windsor;

using Microsoft.Extensions.DependencyInjection;

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
        private DependencyContainer _veloContainer;

        [GlobalSetup]
        public void Init()
        {
            //_autofacContainer = DependencyBuilders.ForAutofac().Build();
            //_castleContainer = DependencyBuilders.ForCastle();
            _coreContainer = DependencyBuilders.ForCore().BuildServiceProvider();
            _veloContainer = DependencyBuilders.ForVelo().BuildContainer();
        }

//        [Benchmark]
//        public string Autofac()
//        {
//            var controller = _autofacContainer.Resolve<DataUserController>();
//            var dataService = _autofacContainer.Resolve<IDataService>();
//            var userService = _autofacContainer.Resolve<IUserService>();
//            return controller.Name + dataService.Name + userService.Name;
//        }
        
//        [Benchmark]
//        public string Castle()
//        {
//            var controller = _castleContainer.Resolve<DataUserController>();
//            var dataService = _castleContainer.Resolve<IDataService>();
//            var userService = _castleContainer.Resolve<IUserService>();
//            return controller.Name + dataService.Name + userService.Name;
//        }
        
        [Benchmark(Baseline = true)]
        public string Core()
        {
            var controller = _coreContainer.GetService<SomethingController>();
            var dataService = _coreContainer.GetService<IDataService>();
            var userService = _coreContainer.GetService<IUserService>();
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
    }
}