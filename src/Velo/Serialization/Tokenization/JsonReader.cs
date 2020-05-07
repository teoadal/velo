using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Velo.Utils;

namespace Velo.Serialization.Tokenization
{
    internal struct JsonReader : IEnumerator<char>
    {
        public char Current { get; private set; }

        public bool CanRead => _canRead;

        private bool _canRead;
        private bool _disposed;
        private TextReader _streamReader;

        public JsonReader(Stream stream, Encoding? encoding = null)
        {
            Current = default;

            _canRead = true;
            _disposed = false;
            _streamReader = new StreamReader(stream, encoding ?? Encoding.UTF8);
        }

        public JsonReader(string source)
        {
            Current = default;

            _canRead = true;
            _disposed = false;
            _streamReader = new StringReader(source);
        }

        public bool MoveNext()
        {
            if (_disposed) throw Error.Disposed(nameof(JsonReader));

            var value = _streamReader.Read();
            if (value == -1)
            {
                _canRead = false;
                return false;
            }

            Current = (char) value;

            return true;
        }

        public void Skip() => MoveNext();

        void IEnumerator.Reset()
        {
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            if (_disposed) return;

            _canRead = false;
            _streamReader.Dispose();
            _streamReader = null!;

            _disposed = true;
        }
    }
}