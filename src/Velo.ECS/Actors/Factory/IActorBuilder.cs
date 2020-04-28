using System;
using System.Linq.Expressions;
using Velo.ECS.Components;
using Velo.Utils;

namespace Velo.ECS.Actors.Factory
{
    public interface IActorBuilder
    {
    }

    public interface IActorBuilder<out TActor> : IActorBuilder
        where TActor : Actor
    {
        TActor Build(int actorId, IComponent[]? components);
    }

    internal sealed class DefaultActorBuilder<TActor> : IActorBuilder<TActor>
        where TActor : Actor
    {
        private readonly Func<int, IComponent[]?, TActor> _builder;

        public DefaultActorBuilder()
        {
            var actorType = typeof(TActor);
            var constructor = ReflectionUtils.GetConstructor(actorType);

            var actorId = Expression.Parameter(typeof(int));
            var components = Expression.Parameter(typeof(IComponent[]));

            _builder = Expression
                .Lambda<Func<int, IComponent[]?, TActor>>(Expression.New(constructor), actorId, components)
                .Compile();
        }

        public TActor Build(int actorId, IComponent[]? components)
        {
            return _builder(actorId, components);
        }
    }
}