using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.ECS
{
    public readonly struct Sign
    {
        // ReSharper disable once InconsistentNaming
        public const int EMPTY_INDEX = -1;
        
        private readonly int _c0;
        private readonly int _c1;
        private readonly int _c2;
        private readonly int _c3;
        private readonly int _c4;

        public Sign(int c0, int c1 = EMPTY_INDEX, int c2 = EMPTY_INDEX, int c3 = EMPTY_INDEX, int c4 = EMPTY_INDEX)
        {
            _c0 = c0;
            _c1 = c1;
            _c2 = c2;
            _c3 = c3;
            _c4 = c4;
        }

        public Sign Add(int typeId, out int index)
        {
            index = IndexOf(EMPTY_INDEX);
            return Set(index, typeId);
        }
        
        public bool Contains(int typeId)
        {
            return IndexOf(typeId) != -1;
        }
        
        public bool ContainsAll(int[] typeIds)
        {
            var containsCount = 0;
            for (var i = 0; i < typeIds.Length; i++)
            {
                ref readonly var typeId = ref typeIds[i];
                if (IndexOf(typeId) != -1) containsCount++;
            }

            return containsCount == typeIds.Length;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(int typeId)
        {
            if (_c0 == typeId) return 0;
            if (_c1 == typeId) return 1;
            if (_c2 == typeId) return 2;
            if (_c3 == typeId) return 3;
            if (_c4 == typeId) return 4;
            return -1;
        }

        public Sign Set(int index, int typeId)
        {
            switch (index)
            {
                case 0:
                    return new Sign(typeId, _c1, _c2, _c3, _c4);
                case 1:
                    return new Sign(_c0, typeId, _c2, _c3, _c4);
                case 2:
                    return new Sign(_c0, _c1, typeId, _c3, _c4);
                case 3:
                    return new Sign(_c0, _c1, _c2, typeId, _c4);
                case 4:
                    return new Sign(_c0, _c1, _c2, _c3, typeId);
            }

            throw Error.OutOfRange();
        }

        public Sign Remove(int typeId, out int index)
        {
            index = IndexOf(typeId);
            return Set(index, EMPTY_INDEX);
        }
    }
}