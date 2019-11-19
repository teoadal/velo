using Velo.CQRS.Queries;
using Velo.TestsModels.Boos;

namespace Velo.TestsModels.Emitting.Boos.Get
{
    public class Query : IQuery<Boo>
    {
        public int Id { get; set; }
    }
}