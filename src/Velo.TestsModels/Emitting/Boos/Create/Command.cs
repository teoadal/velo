namespace Velo.TestsModels.Emitting.Boos.Create
{
    public class Command : IMeasureCommand
    {
        public int Id { get; set; }

        public int Int { get; set; }

        public bool Bool { get; set; }
        
        public bool PostProcessed { get; set; }
        
        public bool PreProcessed { get; set; }
    }
}