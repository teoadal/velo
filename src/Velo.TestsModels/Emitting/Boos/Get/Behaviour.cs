using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Queries;
using Velo.TestsModels.Boos;

namespace Velo.TestsModels.Emitting.Boos.Get
{
    public sealed class Behaviour : IQueryBehaviour<Query, Boo>
    {
        public TimeSpan Elapsed { get; private set; }
        
        public async Task<Boo> Execute(Query query, Func<Task<Boo>> next, CancellationToken cancellationToken)
        {
            var timer = Stopwatch.StartNew();
            
            var result = await next();

            Elapsed = timer.Elapsed;

            return result;
        }
    }
}