using System;
using System.Collections.Generic;
using Velo.ECS.Sources.Context;

namespace Velo.ECS.Sources
{
    public interface IEntitySource<TEntity>: IDisposable
        where TEntity : class, IEntity
    {
        IEnumerable<TEntity> GetEntities(IEntitySourceContext<TEntity> context);
    }
}