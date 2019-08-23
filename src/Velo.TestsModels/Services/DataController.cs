namespace Velo.TestsModels.Services
{
    public class DataController
    {
        public IDataService DataService { get; }

        public IUserService UserService { get; }

        public ILogger Logger { get; }
        
        public DataController(IDataService dataService, IUserService userService, ILogger logger)
        {
            DataService = dataService;
            UserService = userService;
            
            Logger = logger;
        }
    }
}