using System.IO;
using Velo.ECS.Actors.Context;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.ECS.Actors.Sources.Json
{
    internal sealed class ActorContextConverter : JsonConverter<IActorContext>
    {
        private readonly JConverter _converter;

        public ActorContextConverter(JConverter converter) : base(false)
        {
            _converter = converter;
        }

        public override void Serialize(IActorContext value, TextWriter output)
        {
            output.WriteArrayStart();

            var first = true;
            foreach (var actor in value)
            {
                if (first) first = false;
                else output.Write(',');

                _converter.Serialize(actor, output);
            }

            output.WriteArrayEnd();
        }

        public override JsonData Write(IActorContext context)
        {
            var converters = _converter.Converters;

            var actors = new JsonData[context.Length];
            var counter = 0;
            foreach (var actor in context)
            {
                var converter = converters.Get(actor.GetType());
                var actorData = converter.WriteObject(actor);

                actors[counter++] = actorData;
            }

            return new JsonArray(actors);
        }

        public override IActorContext Deserialize(JsonTokenizer _) => throw Error.NotSupported();
        public override IActorContext Read(JsonData _) => throw Error.NotSupported();
    }
}