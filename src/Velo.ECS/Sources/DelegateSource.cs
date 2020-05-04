using System;
using System.Collections.Generic;

namespace Velo.ECS.Sources
{
    internal sealed class DelegateSource<TEntity> : ISource<TEntity>
        where TEntity : class, IEntity
    {
        private readonly Func<ISourceContext<TEntity>, IEnumerable<TEntity>> _builder;

        public DelegateSource(Func<ISourceContext<TEntity>, IEnumerable<TEntity>> builder)
        {
            _builder = builder;
        }

        public IEnumerable<TEntity> GetEntities(ISourceContext<TEntity> context)
        {
            return _builder(context);
        }
    }
}