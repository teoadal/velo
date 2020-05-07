using System.Collections.Generic;

namespace Velo.ECS.Sources.Context
{
    public interface IEntitySourceContext<out TEntity>
        where TEntity : class, IEntity
    {
        bool IsStarted { get; }

        TEntity Get(int id);

        IEnumerable<TEntity> GetEntities();
    }
}