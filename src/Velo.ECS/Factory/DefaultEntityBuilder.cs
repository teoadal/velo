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

            var entityId = Expression.Parameter(typeof(int));
            var components = Expression.Parameter(typeof(IComponent[]));

            _builder = Expression
                .Lambda<Func<int, IComponent[]?, TEntity>>(Expression.New(constructor), entityId, components)
                .Compile();
        }

        public TEntity Build(int entityId, IComponent[]? components)
        {
            return _builder(entityId, components);
        }
    }
}