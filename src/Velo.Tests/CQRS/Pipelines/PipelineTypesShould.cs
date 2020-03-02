using System.Threading.Tasks;
using FluentAssertions;
using Velo.CQRS.Commands;
using Velo.CQRS.Notifications;
using Velo.CQRS.Pipeline;
using Velo.CQRS.Queries;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Create;
using Velo.TestsModels.Emitting.Boos.Get;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS.Pipelines
{
    public class PipelineTypesShould : TestClass
    {
        public PipelineTypesShould(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetCommandPipelineType()
        {
            var pipelineType = PipelineTypes.GetCommandPipelineType(typeof(Command));
            pipelineType.Should().Be<CommandPipeline<Command>>();
        }

        [Fact]
        public void GetNotificationPipelineType()
        {
            var pipelineType = PipelineTypes.GetNotificationPipelineType(typeof(Notification));
            pipelineType.Should().Be<NotificationPipeline<Notification>>();
        }

        [Fact]
        public void GetQueryPipelineType()
        {
            var pipelineType = PipelineTypes.GetQueryPipelineType(typeof(Query));
            pipelineType.Should().Be<QueryPipeline<Query, Boo>>();
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