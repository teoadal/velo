using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Objects;
using Velo.Serialization.Tokenization;

namespace Velo.TestsModels.Serialization
{
    internal sealed class CustomConverter : IPropertyConverter<CustomConverterModel>
    {
        public void Deserialize(JsonTokenizer source, CustomConverterModel instance)
        {
            instance.Value = 1;
        }

        public void Read(JsonObject source, CustomConverterModel instance)
        {
            instance.Value = 1;
        }

        public void Serialize(CustomConverterModel instance, TextWriter output)
        {
            output.Write("one");
        }

        public void Write(CustomConverterModel instance, JsonObject output)
        {
            output.Add("Value", JsonValue.String("one"));
        }
    }
}