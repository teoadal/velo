using System.Collections.Generic;

namespace Velo.ECS.Sources
{
    public interface ISource<TEntity>
        where TEntity : class, IEntity
    {
        IEnumerable<TEntity> GetEntities(ISourceContext<TEntity> context);
    }
}