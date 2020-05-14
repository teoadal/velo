using System.IO;
using System.Text;
using AutoFixture.Xunit2;
using FluentAssertions;
using Newtonsoft.Json;
using Velo.Settings.Sources;
using Velo.TestsModels;
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
        public void ReturnValues()
        {
            var fileSource = new JsonFileSource("Settings/appsettings.json", true);
            fileSource.TryGet(out _).Should().BeTrue();
        }

        [Theory]
        [AutoData]
        public void ReturnValidValues(BigObject values)
        {
            var fileName = $"{nameof(ReturnValidValues)}.json";
            CreateFile(fileName, values);

            try
            {
                var fileSource = new JsonFileSource(fileName, true);
                fileSource.TryGet(out var jsonObject);

                var converter = BuildConvertersCollection().Get<BigObject>();
                converter.Read(jsonObject).Should().BeEquivalentTo(values);
            }
            catch
            {
                File.Delete(fileName);
            }
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

        private static void CreateFile(string fileName, BigObject data)
        {
            var content = JsonConvert.SerializeObject(data);
            File.WriteAllText(fileName, content, Encoding.UTF8);
        }
    }
}