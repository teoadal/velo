using System.IO;
using System.Text;

namespace Velo.Serialization.Tokenization
{
    internal ref struct JsonReader
    {
        public char Current { get; private set; }

        public bool CanRead => _canRead;

        private bool _canRead;
        private readonly TextReader _streamReader;

        public JsonReader(Stream stream, Encoding? encoding = null)
        {
            Current = default;

            _canRead = true;
            _streamReader = new StreamReader(stream, encoding ?? Encoding.UTF8);
        }

        public JsonReader(string source)
        {
            Current = default;

            _canRead = true;
            _streamReader = new StringReader(source);
        }

        public bool MoveNext()
        {
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

        public void Dispose()
        {
            _streamReader.Dispose();
        }
    }
}