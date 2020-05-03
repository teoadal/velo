using Velo.Serialization.Attributes;

namespace Velo.TestsModels.Serialization
{
    public class CustomConverterModel
    {
        [Converter(typeof(CustomConverter))] 
        public int Value { get; set; }
    }
}