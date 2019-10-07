using Velo.Emitting.Queries;

namespace Velo.TestsModels.Boos.Emitting
{
    public class MultipleQueryHandler : IQueryHandler<GetBoo, Boo>, IQueryHandler<GetBooInt, int>
    {
        public bool GetBooCalled { get; private set; }
        public bool GetBooIntCalled { get; private set; }
        
        public Boo Execute(GetBoo query)
        {
            GetBooCalled = true;
            return default;
        }

        public int Execute(GetBooInt query)
        {
            GetBooIntCalled = true;
            return default;
        }
    }
}