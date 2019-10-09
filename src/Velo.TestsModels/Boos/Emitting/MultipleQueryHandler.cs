using System.Threading;
using System.Threading.Tasks;
using Velo.Emitting.Queries;

namespace Velo.TestsModels.Boos.Emitting
{
    public class MultipleQueryHandler : IQueryHandler<GetBoo, Boo>, IQueryHandler<GetBooInt, int>
    {
        public bool GetBooCalled { get; private set; }
        public bool GetBooIntCalled { get; private set; }
        
        public Task<Boo> ExecuteAsync(GetBoo query, CancellationToken cancellationToken)
        {
            GetBooCalled = true;
            return Task.FromResult<Boo>(null);
        }

        public Task<int> ExecuteAsync(GetBooInt query, CancellationToken cancellationToken)
        {
            GetBooIntCalled = true;
            return Task.FromResult(0);
        }
    }
}