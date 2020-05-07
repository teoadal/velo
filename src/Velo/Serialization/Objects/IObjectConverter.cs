using Velo.Serialization.Models;

namespace Velo.Serialization.Objects
{
    internal interface IObjectConverter : IJsonConverter
    {
        object? FillObject(JsonObject jsonData, object instance);
    }

    internal interface IObjectConverter<T> : IObjectConverter, IJsonConverter<T>
    {
        T Fill(JsonObject jsonData, T instance);
    }
}