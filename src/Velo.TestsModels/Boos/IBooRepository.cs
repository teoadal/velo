using System.Threading.Tasks;
using Velo.TestsModels.Domain;

namespace Velo.TestsModels.Boos
{
    public interface IBooRepository : IRepository<Boo>
    {
        Task AddElementAsync(Boo boo);
        
        Task<Boo> GetElementAsync(int id);
    }
}