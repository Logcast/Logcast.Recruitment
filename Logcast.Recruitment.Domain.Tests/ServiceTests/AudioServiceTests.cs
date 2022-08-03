using System;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Entities;
using Logcast.Recruitment.DataAccess.Exceptions;
using Logcast.Recruitment.DataAccess.Repositories;
using Logcast.Recruitment.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Http.Features;
using Moq;

namespace Logcast.Recruitment.Domain.Tests.ServiceTests
{
    [TestClass]
    public class AudioServiceTests
    {
        private readonly Mock<IAudioFileRepository> _audioFileRepositoryMock;
        private readonly Mock<IMetaDataRepository> _metaDataRepositoryMock;
        private readonly IAudioService _audioService;

        public AudioServiceTests()
        {
            _audioFileRepositoryMock = new Mock<IAudioFileRepository>();
            _metaDataRepositoryMock = new Mock<IMetaDataRepository>();
            _audioService = new AudioService(_audioFileRepositoryMock.Object, _metaDataRepositoryMock.Object);
        }

        private async FormFile CreateTestFile(string filepath, string filename)
        {
            var source = File.OpenRead($"{Environment.CurrentDirectory}\\{filepath}\\{filename}");
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(source);
            writer.Flush();
            stream.Position = 0;
            return await new FormFile(stream, 0, stream.Length, "id_from_form", filename);
        }

        [TestMethod]
        public async Task AddAudioFile_NoErrors_VerifyCalls()
        {
            var testFile = CreateTestFile("ServiceTests\\AudioServiceTestFiles", "validfile.mp3");
            var audioDataId = await _audioService.AddAudioFileAsync(testFile);

            Assert.IsNotNull(audioDataId);
            var uploadedAudioData = await _audioService.GetAudioDataAsync(audioDataId);
            Assert.IsNotNull(uploadedAudioData);
            Assert.AreEqual(testFile.filename, uploadedAudioData.Name);
            Assert.IsNotNull(uploadedAudioData.Path);
            Assert.IsNotNull(uploadedAudioData.stream);
            Assert.IsNotNull(uploadedAudioData.Extension);
            var metaDataId = uploadedAudioData.metaDataId;
            Assert.IsNotNull(metaDataId);
            
            Assert.IsNotNull(metaDataId);
            var metaData = await _audioService.GetMetaDataAsync(metaDataId);
            Assert.IsNotNull(metaData);
            Assert.AreEqual("TestTitle", metaData.Title);
            Assert.AreEqual("TestArtist", metaData.Artist);
            Assert.AreEqual("TestAlbum", metaData.Album);
            Assert.AreEqual("TestAlbumArtists", metaData.AlbumArtist);
            Assert.AreEqual("TestGenre", metaData.Genre);
            Assert.AreEqual(1, metaData.TrackNumber);
            Assert.AreEqual(256, metaData.Bitrate);
            Assert.AreEqual(10000, metaData.Duration);
            Assert.AreEqual(testFile.Path, metaData.FilePath);

            _audioFileRepositoryMock.Verify(a => a.AddAudioFileAsync(testFile));
        }

        [TestMethod]
        public async Task AddAudioFile_InvalidFile_VerifyCalls()
        {
            var testFile = CreateTestFile("ServiceTests\\AudioServiceTestFiles", "invalidfile.mp3");
            Assert.ThrowsExceptionAsync<InvalidFileException>(await _audioService.AddAudioFileAsync(testFile));
        }

        [TestMethod]
        public async Task AddAudioFile_InvalidFileType_VerifyCalls()
        {
            var testFile = CreateTestFile("ServiceTests\\AudioServiceTestFiles", "invalidfiletype.txt");
            Assert.ThrowsExceptionAsync<InvalidFileException>(await _audioService.AddAudioFileAsync(testFile));
        }

        [TestMethod]
        public async Task GetAudioData_IsThere_VerifyCalls()
        {
            var testFile = CreateTestFile("ServiceTests\\AudioServiceTestFiles", "validfile.mp3");
            var audioDataID = await _audioFileRepositoryMock.Setup(a => a.AddAudioFileAsync(testFile));

            Assert.IsNotNull(audioDataID);
            Assert.IsInstanceOfType(_audioService.GetAudioDataAsync(audioDataID), typeof(AudioData));
            Assert.AreEqual(audioDataID, _audioService.AddAudioFileAsync(testFile));

            _audioFileRepositoryMock.Verify(a => a.AddAudioFileAsync(testFile));
        }

        [TestMethod]
        public async Task GetAudioData_IsMissing_VerifyCalls()
        {
            Assert.ThrowsExceptionAsync<UnableToSaveAudioException>(await _audioService.GetAudioDataAsync(-1));
        }
    }
}