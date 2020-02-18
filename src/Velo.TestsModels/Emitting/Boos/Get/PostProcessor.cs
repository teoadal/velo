using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Queries;
using Velo.TestsModels.Boos;

namespace Velo.TestsModels.Emitting.Boos.Get
{
    public class PostProcessor : IQueryPostProcessor<Query, Boo>
    {
        public Task PostProcess(Query query, Boo result, CancellationToken cancellationToken)
        {
            query.PostProcessed = true;
            return Task.CompletedTask;
        }
    }
}