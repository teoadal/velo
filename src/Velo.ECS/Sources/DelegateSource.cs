using System;
using System.Collections.Generic;
using Velo.ECS.Sources.Context;

namespace Velo.ECS.Sources
{
    internal sealed class DelegateSource<TEntity> : IEntitySource<TEntity>
        where TEntity : class, IEntity
    {
        private readonly Func<IEntitySourceContext<TEntity>, IEnumerable<TEntity>> _builder;

        public DelegateSource(Func<IEntitySourceContext<TEntity>, IEnumerable<TEntity>> builder)
        {
            _builder = builder;
        }

        public IEnumerable<TEntity> GetEntities(IEntitySourceContext<TEntity> context)
        {
            return _builder(context);
        }

        public void Dispose()
        {
        }
    }
}