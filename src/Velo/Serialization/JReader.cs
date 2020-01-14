using System.IO;
using System.Text;

namespace Velo.Serialization
{
    internal ref struct JReader
    {
        public char Current { get; private set; }

        private readonly TextReader _streamReader;

        public JReader(Stream stream, Encoding encoding)
        {
            _streamReader = new StreamReader(stream, encoding);
            Current = default;
        }

        public JReader(string source)
        {
            _streamReader = new StringReader(source);
            Current = default;
        }

        public bool MoveNext()
        {
            var value = _streamReader.Read();
            if (value == -1) return false;

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