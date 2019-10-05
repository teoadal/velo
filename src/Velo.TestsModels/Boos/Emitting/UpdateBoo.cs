using Velo.Emitting.Commands;

namespace Velo.TestsModels.Boos.Emitting
{
    public class UpdateBoo : ICommand
    {
        public int Id { get; set; }
        
        public int Int { get; set; }

        public bool Bool { get; set; }
    }
}