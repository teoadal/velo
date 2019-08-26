namespace Velo.TestsModels.Services
{
    public class SomethingController
    {
        public string Name { get; }

        public IFooService FooService { get; }

        public IBooService BooService { get; }

        public ILogger Logger { get; }

        public SomethingController(IFooService fooService, IBooService booService, ILogger logger)
        {
            Name = fooService.Repository.Session.Id.ToString("N");

            FooService = fooService;
            BooService = booService;

            Logger = logger;
        }
    }
}