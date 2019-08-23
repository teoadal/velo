namespace Velo.TestsModels.Services
{
    public interface IRepository
    {
        IConfiguration Configuration { get; }

        ISession Session { get; }

    }
}