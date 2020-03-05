using System.Threading.Tasks;
using FluentAssertions;
using Velo.CQRS;
using Velo.CQRS.Commands.Pipeline;
using Velo.CQRS.Notifications.Pipeline;
using Velo.CQRS.Queries.Pipeline;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Create;
using Velo.TestsModels.Emitting.Boos.Get;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS
{
    public class TypesShould : TestClass
    {
        public TypesShould(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetCommandPipelineType()
        {
            var pipelineType = Types.GetCommandPipelineType(typeof(Command));
            pipelineType.Should().Be<ICommandPipeline<Command>>();
        }

        [Fact]
        public void GetNotificationPipelineType()
        {
            var pipelineType = Types.GetNotificationPipelineType(typeof(Notification));
            pipelineType.Should().Be<INotificationPipeline<Notification>>();
        }

        [Fact]
        public void GetQueryPipelineType()
        {
            var pipelineType = Types.GetQueryPipelineType(typeof(Query));
            pipelineType.Should().Be<IQueryPipeline<Query, Boo>>();
        }

        [Fact]
        public void ThreadSafe()
        {
            Parallel.For(0, 100, _ =>
            {
                GetCommandPipelineType();
                GetNotificationPipelineType();
                GetQueryPipelineType();
            });
        }
    }
}