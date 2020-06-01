using System.Collections.Generic;

namespace Velo.ECS.Sources.Context
{
    public interface IEntitySourceContext<TEntity>
        where TEntity : class, IEntity
    {
        bool IsStarted { get; }

        TEntity Get(int id);

        IEnumerable<TEntity> GetEntities(IEntitySource<TEntity>[] sources);
    }
}