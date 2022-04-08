using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Repositories;
using Logcast.Recruitment.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Logcast.Recruitment.Domain.Tests.ServiceTests
{
    [TestClass]
    public class FileServiceTests
    {
        private readonly Mock<IFileRepository> _fileRepositoryMock;
        private readonly IFileService _fileService;
        private string _fileToDiscard = string.Empty;

        public FileServiceTests()
        {
            _fileRepositoryMock = new Mock<IFileRepository>();
            _fileService = new FileService(_fileRepositoryMock.Object);
        }

        [TestMethod]
        public async Task SendAudioFileAsync_NoErrors_VerifyCalls()
        {
            // Arrange.
            var fileMock = new Mock<IFormFile>();
            var source = File.OpenRead($"{Environment.CurrentDirectory}\\ServiceTests\\TestFiles\\file_example.mp3");
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(source);
            writer.Flush();
            ms.Position = 0;
            var fileName = "file_example.mp3";
            fileMock.Setup(f => f.FileName).Returns(fileName).Verifiable();
            fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) => ms.CopyToAsync(stream))
                .Verifiable();

            // Assert
            try
            {
                var res = (await _fileService.AddFileAsync(fileMock.Object));
                Assert.IsNotNull(res);

                _fileToDiscard = res.Item3;
            }
            catch (Exception) { }       
        }

        [TestCleanup]
        public void Cleanup()
        {
            bool fileExists = File.Exists(_fileToDiscard);
            if (fileExists)
            {
                try
                {
                    File.Delete(_fileToDiscard);
                }
                catch (Exception) { }
            }
        }

        [TestMethod]
        public async Task SaveNotAudioFileAsync_ThrowsException()
        {
            //Arrange
            var fileMock = new Mock<IFormFile>();
            byte[] bytes = null;
            using (var ms = new MemoryStream())
            {
                using (TextWriter tw = new StreamWriter(ms))
                {
                    tw.Write("test");
                    tw.Flush();
                    ms.Position = 0;
                    bytes = ms.ToArray();
                }

                fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
                fileMock.Setup(_ => _.FileName).Returns("test.pdf");
            }

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _fileService.AddFileAsync(fileMock.Object));
        }
    }
}
