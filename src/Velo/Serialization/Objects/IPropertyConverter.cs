using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Objects
{
    internal interface IPropertyConverter<in TOwner>
    {
        void Deserialize(JsonTokenizer source, TOwner instance);

        void Read(JsonObject source, TOwner instance);

        void Serialize(TOwner instance, TextWriter output);

        void Write(TOwner instance, JsonObject output);
    }
}