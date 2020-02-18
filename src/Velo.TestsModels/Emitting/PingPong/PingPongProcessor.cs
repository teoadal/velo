using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Queries;

namespace Velo.TestsModels.Emitting.PingPong
{
    public sealed class PingPongProcessor : IQueryProcessor<Ping, Pong>
    {
        public Task<Pong> Process(Ping query, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Pong(query.Message + 1));
        }
    }
}