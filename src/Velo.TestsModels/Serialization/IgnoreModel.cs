using Velo.Serialization.Attributes;

namespace Velo.TestsModels.Serialization
{
    public class IgnoreModel
    {
        [Ignore]
        public int Ignored { get; set; }
        
        public int Value { get; set; }
    }
}