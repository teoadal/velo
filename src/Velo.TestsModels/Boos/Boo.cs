using System.Collections.Generic;

namespace Velo.TestsModels.Boos
{
    public class Boo
    {
        public int Id { get; set; }

        public bool Bool { get; set; }

        public float Float { get; set; }

        public int Int { get; set; }

        public double Double { get; set; }
        
        public List<int> Values { get; set; }
        
        public ModelType Type { get; set; }
    }
}