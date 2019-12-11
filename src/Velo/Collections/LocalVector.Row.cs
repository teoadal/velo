namespace Velo.Collections
{
    // ReSharper disable once UnusedTypeParameter
    public ref partial struct LocalVector<T>
    {
        private readonly struct Row<TKey, TValue>
        {
            public readonly TKey Key;
            public readonly TValue Value;

            public Row(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}