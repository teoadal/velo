using Velo.Emitting.Queries;

namespace Velo.TestsModels.Boos.Emitting
{
    public class GetBooInt : IQuery<int>
    {
        public int Id { get; set; }
    }
}