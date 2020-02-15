using Velo.CQRS.Queries;

namespace Velo.TestsModels.Emitting.PingPong
{
    public readonly struct Ping: IQuery<Pong>
    {
        public readonly int Message;

        public Ping(int message)
        {
            Message = message;
        }
    }
}