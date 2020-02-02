using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.ECS
{
    public readonly struct Sign
    {
        // ReSharper disable once InconsistentNaming
        public const int EMPTY_INDEX = -1;

        public readonly int Length;

        private readonly int _c0;
        private readonly int _c1;
        private readonly int _c2;
        private readonly int _c3;
        private readonly int _c4;

        internal Sign(int c0)
        {
            _c0 = c0;
            _c1 = EMPTY_INDEX;
            _c2 = EMPTY_INDEX;
            _c3 = EMPTY_INDEX;
            _c4 = EMPTY_INDEX;

            Length = 1;
        }

        internal Sign(int c0, int c1)
        {
            _c0 = c0;
            _c1 = c1;
            _c2 = EMPTY_INDEX;
            _c3 = EMPTY_INDEX;
            _c4 = EMPTY_INDEX;

            Length = 2;
        }

        internal Sign(int c0, int c1, int c2)
        {
            _c0 = c0;
            _c1 = c1;
            _c2 = c2;
            _c3 = EMPTY_INDEX;
            _c4 = EMPTY_INDEX;

            Length = 3;
        }

        internal Sign(int c0, int c1, int c2, int c3)
        {
            _c0 = c0;
            _c1 = c1;
            _c2 = c2;
            _c3 = c3;
            _c4 = EMPTY_INDEX;

            Length = 4;
        }

        internal Sign(int c0, int c1, int c2, int c3, int c4)
        {
            _c0 = c0;
            _c1 = c1;
            _c2 = c2;
            _c3 = c3;
            _c4 = c4;

            Length = 5;
        }

        internal Sign Add(int typeId, out int index)
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
            if (Length < typeIds.Length) return false;
            
            var containsCount = 0;
            foreach (var typeId in typeIds)
            {
                if (IndexOf(typeId) != -1) containsCount++;
            }

            return containsCount == typeIds.Length;
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

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

        public bool TryRemove(int typeId, out Sign newSign, out int oldIndex)
        {
            oldIndex = IndexOf(typeId);
            if (oldIndex == EMPTY_INDEX)
            {
                newSign = default;
                oldIndex = default;
                return false;
            }

            newSign = Set(oldIndex, EMPTY_INDEX);
            return true;
        }
        
        public ref struct Enumerator
        {
            public int Current => _current;

            private int _current;
            private int _position;
            private Sign _sign;

            internal Enumerator(Sign sign)
            {
                _current = EMPTY_INDEX;
                _sign = sign;
                _position = -1;
            }
            
            public bool MoveNext()
            {
                _position++;
                switch (_position)
                {
                    case 0:
                        _current = _sign._c0;
                        break;
                    case 1:
                        _current = _sign._c1;
                        break;
                    case 2:
                        _current = _sign._c2;
                        break;
                    case 3:
                        _current = _sign._c3;
                        break;
                    case 4:
                        _current = _sign._c4;
                        break;
                    default:
                        return false;
                }

                return _current != EMPTY_INDEX;
            }
        }
    }
}