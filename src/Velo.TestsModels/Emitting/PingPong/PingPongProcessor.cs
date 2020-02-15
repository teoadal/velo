using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Queries;

namespace Velo.TestsModels.Emitting.PingPong
{
    public sealed class PingPongProcessor : IQueryProcessor<Ping, Pong>
    {
        public ValueTask<Pong> Process(Ping query, CancellationToken cancellationToken)
        {
            return new ValueTask<Pong>(new Pong(query.Message + 1));
        }
    }
}