using System.IO;
using FluentAssertions;
using Velo.Settings.Sources;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Settings.Sources
{
    public class JsonFileSourceShould : TestClass
    {
        public JsonFileSourceShould(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void NotThrowIfNotRequiredFileNotFound()
        {
            var fileSource = new JsonFileSource("abc.json", false);

            fileSource
                .Invoking(source => source.TryGet(out _))
                .Should().NotThrow<FileNotFoundException>();
        }

        [Fact]
        public void ThrowIfRequiredFileNotFound()
        {
            var fileSource = new JsonFileSource("abc.json", true);

            fileSource
                .Invoking(source => source.TryGet(out _))
                .Should().Throw<FileNotFoundException>();
        }
    }
}