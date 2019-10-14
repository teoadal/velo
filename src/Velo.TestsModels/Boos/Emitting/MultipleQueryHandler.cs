using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Queries;

namespace Velo.TestsModels.Boos.Emitting
{
    public class MultipleQueryHandler : IQueryHandler<GetBoo, Boo>, IQueryHandler<GetBooInt, int>
    {
        public bool GetBooCalled { get; private set; }
        public bool GetBooIntCalled { get; private set; }
        
        public Task<Boo> Handle(GetBoo query, CancellationToken cancellationToken)
        {
            GetBooCalled = true;
            return Task.FromResult<Boo>(null);
        }

        public Task<int> Handle(GetBooInt query, CancellationToken cancellationToken)
        {
            GetBooIntCalled = true;
            return Task.FromResult(0);
        }
    }
}