using System.Threading;
using System.Threading.Tasks;
using Velo.Emitting;
using Velo.Emitting.Commands;

namespace Velo.TestsModels.Boos.Emitting
{
    public class UpdateBooHandler : IAsyncCommandHandler<UpdateBoo>
    {
        private readonly IBooRepository _booRepository;

        public UpdateBooHandler(IBooRepository booRepository)
        {
            _booRepository = booRepository;
        }

        public Task ExecuteAsync(HandlerContext<UpdateBoo> context, CancellationToken cancellationToken)
        {
            var payload = context.Payload;
            return Task.Run(() => _booRepository.UpdateElement(payload.Id, boo =>
            {
                boo.Bool = payload.Bool;
                boo.Int = payload.Int;
            }), cancellationToken);
        }
    }
}