using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Velo.ECS.Assets.Sources
{
    internal sealed class MemoryAssetSource : IAssetSource, IEnumerable<Asset>, IEnumerator<Asset>
    {
        public Asset Current { get; private set; }

        private Asset[] _assets;
        private int _position;

        public MemoryAssetSource(IEnumerable<Asset> assets)
        {
            _assets = assets.ToArray();

            Current = null!;
            _position = -1;
        }

        public IEnumerable<Asset> GetAssets() => this;

        public bool MoveNext()
        {
            _position++;

            if (_position == _assets.Length) return false;

            Current = _assets[_position];

            return true;
        }

        IEnumerator<Asset> IEnumerable<Asset>.GetEnumerator() => this;

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