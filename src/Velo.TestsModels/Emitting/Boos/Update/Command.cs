using Velo.CQRS.Commands;

namespace Velo.TestsModels.Emitting.Boos.Update
{
    public class Command : ICommand
    {
        public int Id { get; set; }
        
        public int Int { get; set; }

        public bool Bool { get; set; }
    }
}