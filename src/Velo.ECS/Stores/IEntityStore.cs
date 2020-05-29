using System.Collections.Generic;

namespace Velo.ECS.Stores
{
    public interface IEntityStore<in TEntity>
        where TEntity: class, IEntity
    {
        void Write(IEnumerable<TEntity> entities);
    }
}