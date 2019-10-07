namespace Velo.TestsModels.Boos.Emitting
{
    public class UpdateBoo : IPolymorphicCommand
    {
        public int Id { get; set; }
        
        public int Int { get; set; }

        public bool Bool { get; set; }
    }
}