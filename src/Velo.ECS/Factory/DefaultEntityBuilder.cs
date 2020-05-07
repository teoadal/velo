using System;
using System.Linq.Expressions;
using Velo.ECS.Components;
using Velo.Utils;

namespace Velo.ECS.Factory
{
    internal abstract class DefaultEntityBuilder<TEntity>
        where TEntity : class, IEntity
    {
        private readonly Func<int, IComponent[]?, TEntity> _builder;

        protected DefaultEntityBuilder()
        {
            var entityType = typeof(TEntity);
            var constructor = ReflectionUtils.GetConstructor(entityType);

            if (constructor == null)
            {
                throw Error.NotFound($"Not found constructor for type {ReflectionUtils.GetName<TEntity>()}");
            }

            var entityId = Expression.Parameter(typeof(int));
            var components = Expression.Parameter(typeof(IComponent[]));

            var body = Expression.New(constructor, entityId, components);
            _builder = Expression
                .Lambda<Func<int, IComponent[]?, TEntity>>(body, entityId, components)
                .Compile();
        }

        public TEntity Build(int entityId, IComponent[]? components)
        {
            return _builder(entityId, components);
        }
    }
}