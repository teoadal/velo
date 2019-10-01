using Velo.CQRS.Queries;

namespace Velo.TestsModels.Boos
{
    public class GetBoo : IQuery<Boo>
    {
        public int Id { get; set; }
    }
}