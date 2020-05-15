using System.IO;
using System.Reflection;
using Velo.ECS.Actors.Context;
using Velo.ECS.Sources;
using Velo.ECS.Sources.Context;
using Velo.ECS.Sources.Json.References;
using Velo.Serialization.Models;
using Velo.Serialization.Objects;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.ECS.Actors.Sources.Json
{
    internal sealed class ActorReferencesConverter<TOwner, TActor> : IPropertyConverter<TOwner>
        where TOwner : class where TActor : Actor
    {
        private readonly IActorContext _actorContext;
        private readonly IEntitySourceContext<Actor> _sources;

        private readonly ReferenceResolver<TOwner, TActor> _resolver;

        public ActorReferencesConverter(
            IActorContext actorContext,
            SourceDescriptions descriptions,
            IEntitySourceContext<Actor> sources,
            PropertyInfo property)
        {
            _actorContext = actorContext;
            _resolver = ReferenceResolver<TOwner, TActor>.Build(property, descriptions, GetEntity);
            _sources = sources;
        }

        public void Read(JsonObject source, TOwner instance)
        {
            _resolver.Read(source, instance);
        }

        public void Serialize(TOwner instance, TextWriter output)
        {
            _resolver.Serialize(instance, output);
        }

        public void Write(TOwner instance, JsonObject output)
        {
            _resolver.Write(instance, output);
        }

        private TActor GetEntity(int id)
        {
            var asset = _sources.IsStarted
                ? _sources.Get(id)
                : _actorContext.Get(id);

            return (TActor) asset;
        }

        void IPropertyConverter<TOwner>.Deserialize(JsonTokenizer _, TOwner instance) => throw Error.NotSupported();
    }
}