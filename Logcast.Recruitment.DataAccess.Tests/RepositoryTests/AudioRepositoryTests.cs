using System;
using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Entities;
using Logcast.Recruitment.DataAccess.Exceptions;
using Logcast.Recruitment.DataAccess.Factories;
using Logcast.Recruitment.DataAccess.Repositories;
using Logcast.Recruitment.DataAccess.Services;
using Logcast.Recruitment.DataAccess.Tests.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Logcast.Recruitment.DataAccess.Tests.RepositoryTests
{
    [TestClass]
    public class AudioRepositoryTests
    {
        private readonly Mock<IFileStorage> _fileStorageMock;
        private readonly IAudioRepository _audioRepository;
        private readonly ApplicationDbContext _testDbContext;

        public AudioRepositoryTests()
        {
            var dbContextFactoryMock = new Mock<IDbContextFactory>();

            _testDbContext = EfConfig.CreateInMemoryTestDbContext();
            dbContextFactoryMock.Setup(d => d.Create()).Returns(EfConfig.CreateInMemoryApplicationDbContext());

            _fileStorageMock = new Mock<IFileStorage>();

            _audioRepository = new AudioRepository(dbContextFactoryMock.Object, _fileStorageMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _testDbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task StoreAudioFileAsync_AudioIdExists_ShouldThrowException()
        {
            var audioId = Guid.NewGuid();
            await _testDbContext.Audio.AddAsync(new Audio(audioId, "name", ".mp3", "audio/mpeg"));
            await _testDbContext.SaveChangesAsync();

            await Assert.ThrowsExceptionAsync<ArgumentException>(()
                => _audioRepository.StoreAudioFileAsync(Array.Empty<byte>(), audioId, "name", ".mp3", "audio/mpeg"));
        }

        [TestMethod]
        public async Task StoreAudioFileAsync_NewAudioId_ShouldCreateNewAudio()
        {
            var audioId = Guid.NewGuid();

            await _audioRepository.StoreAudioFileAsync(Array.Empty<byte>(), audioId, "name", ".mp3", "audio/mpeg");

            var audio = await _testDbContext.Audio.SingleAsync();

            Assert.AreEqual(audioId, audio.Id);
            Assert.AreEqual("name", audio.Name);
            Assert.AreEqual(".mp3", audio.FileType);
            Assert.AreEqual($"{audioId}.mp3", audio.FileName);
            Assert.AreEqual("audio/mpeg", audio.ContentType);
            Assert.IsNull(audio.Creator);
            Assert.IsTrue(_fileStorageMock.Invocations.Count == 1);
        }

        [TestMethod]
        public async Task StoreAudioMetadataAsync_AudioDoesNotExist_ShouldThrowException()
        {
            var audioId = Guid.NewGuid();

            await Assert.ThrowsExceptionAsync<AudioNotFoundException>(()
                => _audioRepository.StoreAudioMetadataAsync(audioId, "Custom name", "John Smith"));
        }

        [TestMethod]
        public async Task StoreAudioMetadataAsync_AudioExists_ShouldUpdateAudioMetadata()
        {
            var audioId = Guid.NewGuid();
            await _testDbContext.Audio.AddAsync(new Audio(audioId, "name", ".mp3", "audio/mpeg"));
            await _testDbContext.SaveChangesAsync();

            await _audioRepository.StoreAudioMetadataAsync(audioId, "Custom name", "John Smith");

            var audio = await _testDbContext.Audio.SingleAsync();

            Assert.AreEqual(audioId, audio.Id);
            Assert.AreEqual("Custom name", audio.Name);
            Assert.AreEqual(".mp3", audio.FileType);
            Assert.AreEqual($"{audioId}.mp3", audio.FileName);
            Assert.AreEqual("audio/mpeg", audio.ContentType);
            Assert.AreEqual("John Smith", audio.Creator);
        }
    }
}
