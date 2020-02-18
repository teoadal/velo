using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Queries;
using Velo.TestsModels.Boos;

namespace Velo.TestsModels.Emitting.Boos.Get
{
    public class PreProcessor : IQueryPreProcessor<Query, Boo>
    {
        public Task PreProcess(Query query, CancellationToken cancellationToken)
        {
            query.PreProcessed = true;
            return Task.CompletedTask;
        }
    }
}