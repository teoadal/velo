using Velo.CQRS.Commands;

namespace Velo.TestsModels.Emitting.Foos.Create
{
    public class Command : ICommand
    {
        public int Id { get; set; }
        
        public ModelType Type { get; set; }
    }
}