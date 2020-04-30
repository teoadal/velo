using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Velo.ECS.Sources
{
    internal abstract class MemorySource<TEntity> : IEnumerable<TEntity>, IEnumerator<TEntity>
        where TEntity : class
    {
        public TEntity Current { get; private set; }

        private TEntity[] _assets;
        private int _position;

        protected MemorySource(IEnumerable<TEntity> assets)
        {
            _assets = assets.ToArray();

            Current = null!;
            _position = -1;
        }

        public bool MoveNext()
        {
            _position++;

            if (_position == _assets.Length) return false;

            Current = _assets[_position];

            return true;
        }

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => this;

        void IEnumerator.Reset()
        {
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            Array.Clear(_assets, 0, _assets.Length);
            _assets = null!;
        }
    }
}