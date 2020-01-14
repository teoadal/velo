using System;
using AutoFixture.Xunit2;
using Xunit;

namespace Velo
{
    public class SpanTests
    {
        [Theory, AutoData]
        public void StackallocSpan(string source)
        {
            Span<char> chars = stackalloc char[source.Length];
            for (var i = 0; i < chars.Length; i++)
            {
                chars[i] = source[i];
            }
            
            var result = new string(chars);
            Assert.Equal(source, result);
        }
        
        [Theory, AutoData]
        public void StackallocSpan_Partial(string source)
        {
            Span<char> chars = stackalloc char[source.Length];
            var length = 5;
            for (var i = 0; i < length; i++)
            {
                chars[i] = source[i];
            }
            
            var result = new string(chars.Slice(0, length));
            Assert.Equal(source.Substring(0, length), result);
        }
    }
}