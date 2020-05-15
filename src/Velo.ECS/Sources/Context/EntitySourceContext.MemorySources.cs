namespace Velo.ECS.Sources.Context
{
    internal sealed partial class EntitySourceContext<TEntity>
    {
        private sealed class MemorySources : IReference<IEntitySource<TEntity>[]>
        {
            public IEntitySource<TEntity>[] Value { get; }

            public MemorySources(IEntitySource<TEntity>[] sources)
            {
                Value = sources;
            }

            public void Dispose()
            {
            }
        }
    }
}