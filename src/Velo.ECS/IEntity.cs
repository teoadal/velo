using System.Collections.Generic;
using Velo.ECS.Components;

namespace Velo.ECS
{
    public interface IEntity
    {
        public int Id { get; }

        public IEnumerable<IComponent> Components { get; }
    }
}