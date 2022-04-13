using System;
using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Exceptions;
using Logcast.Recruitment.DataAccess.Repositories;
using Logcast.Recruitment.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Logcast.Recruitment.Domain.Tests.ServiceTests
{
    [TestClass]
    public class AudioServiceTests
    {
        private readonly Mock<IAudioRepository> _audioRepositoryMock;
        private readonly Mock<IIdGenerator> _idGeneratorMock;
        private readonly IAudioService _audioService;
        private readonly Guid _guid = Guid.NewGuid();

        public AudioServiceTests()
        {
            _audioRepositoryMock = new Mock<IAudioRepository>();
            _idGeneratorMock = new Mock<IIdGenerator>();
            _idGeneratorMock.Setup(e => e.NewId()).Returns(_guid);
            _audioService = new AudioService(_audioRepositoryMock.Object, _idGeneratorMock.Object);
        }

        [TestMethod]
        public async Task StoreAudioFileAsync_NoErrors_VerifyCalls()
        {
            var file = Array.Empty<byte>();
            var name = "name";
            var fileType = ".mp3";
            var contentType = "audio/mpeg";
            await _audioService.StoreAudioFileAsync(file, $"{name}{fileType}", contentType);

            _audioRepositoryMock.Verify(a => a.StoreAudioFileAsync(file, _guid, name, fileType, contentType));
        }

        [TestMethod]
        public async Task StoreAudioFileAsync_InvalidContentType_ShouldThrowException()
        {
            var file = Array.Empty<byte>();
            var name = "name";
            var fileType = ".mp3";
            var contentType = "audio";

            await Assert.ThrowsExceptionAsync<UnsupportedContentTypeException>(() => _audioService.StoreAudioFileAsync(file, $"{name}{fileType}", contentType));

            Assert.AreEqual(0, _audioRepositoryMock.Invocations.Count);
        }
    }
}
