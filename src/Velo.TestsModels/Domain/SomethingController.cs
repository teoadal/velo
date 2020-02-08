using Velo.Logging;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;

namespace Velo.TestsModels.Domain
{
    public class SomethingController
    {
        public string Name { get; }

        public IFooService FooService { get; }

        public IBooService BooService { get; }

        public ILogger<SomethingController> Logger { get; }

        public SomethingController(IFooService fooService, IBooService booService, ILogger<SomethingController> logger)
        {
            Name = fooService.Repository.Session.Id.ToString("N");

            FooService = fooService;
            BooService = booService;

            Logger = logger;
        }
    }
}