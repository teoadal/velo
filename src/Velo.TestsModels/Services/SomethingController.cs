namespace Velo.TestsModels.Services
{
    public class SomethingController
    {
        public string Name { get; }
        
        public IDataService DataService { get; }

        public IUserService UserService { get; }

        public ILogger Logger { get; }
        
        public SomethingController(IDataService dataService, IUserService userService, ILogger logger)
        {
            Name = dataService.Repository.Session.Id.ToString("N");
            
            DataService = dataService;
            UserService = userService;
            
            Logger = logger;
        }
    }
}