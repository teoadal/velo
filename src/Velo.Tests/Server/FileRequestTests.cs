using System;
using System.IO;
using System.Net;
using System.Text;
using Velo.TestsModels;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Server
{
    public class FileRequestTests : TestClass, IClassFixture<ServerFixture>
    {
        private readonly ServerFixture _fixture;

        public FileRequestTests(ServerFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        [Fact]
        public void GetFile()
        {
            var fileName = $"{nameof(GetFile)}.txt";

            const string expectedFileData = "TEST";
            CreateFile(fileName, expectedFileData);

            var response = Measure(() => CreateRequest(fileName));
            Assert.Equal(expectedFileData, response);

            File.Delete(fileName);
        }
        
        [Fact]
        public void GetFile_FromDirectory()
        {
            var fileName = $"{nameof(GetFile_FromDirectory)}.txt";
            var directory = Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "web")).FullName;
            var filePath = Path.Combine(directory, fileName);

            const string expectedFileData = "TEST";
            CreateFile(filePath, expectedFileData);
            
            var response = Measure(() => CreateRequest("web/" + fileName));
            Assert.Equal(expectedFileData, response);

            File.Delete(filePath);
        }

        [Fact]
        public void Throw_Method_NotFound()
        {
            Assert.Throws<WebException>(() => CreateRequest("controller/users"));
        }

        [Fact]
        public void Throw_File_NotFound()
        {
            Assert.Throws<WebException>(() => CreateRequest("not_found.txt"));
        }

        private static void CreateFile(string path, string data)
        {
            using var fileStream = File.Create(path);
            fileStream.Write(Encoding.UTF8.GetBytes(data));
        }
        
        private string CreateRequest(string path)
        {
            var request = WebRequest.Create($"http://localhost:{_fixture.Port}/{path}");
            using var response = request.GetResponse();
            using var stream = response.GetResponseStream();
            using var streamReader = new StreamReader(stream ?? throw new NullReferenceException());

            return streamReader.ReadToEnd();
        }
    }
}