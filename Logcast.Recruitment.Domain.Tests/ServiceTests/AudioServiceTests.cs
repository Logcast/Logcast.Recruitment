using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HashidsNet;
using Logcast.Recruitment.DataAccess.Repositories;
using Logcast.Recruitment.Domain.Services;
using Logcast.Recruitment.Shared.Exceptions;
using Logcast.Recruitment.Shared.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Logcast.Recruitment.Domain.Tests.ServiceTests
{   [TestClass]
	public class AudioServiceTests
	{
		private readonly Mock<IAudioRepository> _audioRepositoryMock;
		private readonly Mock<IMetadataRepository> _metadataRepositoryMock;
		private readonly IFileValidatorService _fileValidatorService;
		private readonly AudioService _audioService;
		

		public AudioServiceTests()
		{
			_audioRepositoryMock = new Mock<IAudioRepository>();
			_metadataRepositoryMock = new Mock<IMetadataRepository>();
			_fileValidatorService = new FileValidatorService();
			_audioService = new AudioService(_audioRepositoryMock.Object, _metadataRepositoryMock.Object, _fileValidatorService, new Hashids());
		}
		[TestMethod]
		[DeploymentItem(@"Audio\white-noise.mp3")]
		public async Task CreateAudioFile_ValidFile_VerifyCalls()
		{
			_audioRepositoryMock.Setup(e => e.CreateAudioAsync(It.IsAny<Stream>(), "white-noise.mp3", new CancellationToken())).ReturnsAsync(0);
			var fs = new FileStream(@"../../../ServiceTests\Audio\white-noise.mp3", FileMode.Open);
			await _audioService.CreateAudioFileAsync(fs, "white-noise.mp3", new CancellationToken());

			_audioRepositoryMock.Verify(a => a.CreateAudioAsync(It.IsAny<Stream>(), "white-noise.mp3", new CancellationToken()), Times.Once);

		}
		
		[TestMethod]
		[DeploymentItem(@"Audio\white-noise-corrupt.mp3")]
		public async Task CreateAudioFile_InvalidFile_NoCalls()
		{
			var fs = new FileStream(@"..\..\..\ServiceTests\Audio\white-noise-corrupt.mp3", FileMode.Open);

			await Assert.ThrowsExceptionAsync<ValidationFailedException>(() =>
			{
				return _audioService.CreateAudioFileAsync(fs, "white-noise-corrupt.mp3", new CancellationToken());
			});
			_audioRepositoryMock.Verify(a => a.CreateAudioAsync(It.IsAny<Stream>(), "white-noise-corrupt.mp3", new CancellationToken()), Times.Never);
		}
		[TestMethod]
		[DeploymentItem(@"Audio\white-noise.mp3")]
		[DoNotParallelize]
		public async Task CreateAudioFile_ValidFile_VerifySuggestedMetadata()
		{
			_audioRepositoryMock.Setup(e => e.CreateAudioAsync(It.IsAny<Stream>(), "white-noise.mp3", new CancellationToken())).ReturnsAsync(0);
			var ms = new MemoryStream();
			new FileStream(@"..\..\..\ServiceTests\Audio\white-noise.mp3", FileMode.Open).CopyTo(ms);

			var fileUpload = await _audioService.CreateAudioFileAsync(ms, "white-noise.mp3", new CancellationToken());

			Assert.AreEqual("Test", fileUpload.SuggestedMetadataModel.Title);
			Assert.AreEqual("Test", fileUpload.SuggestedMetadataModel.Performers);
			Assert.AreEqual("Test", fileUpload.SuggestedMetadataModel.Album);
			Assert.AreEqual(128, fileUpload.SuggestedMetadataModel.AudioBitrate);
			Assert.AreEqual("audio/mp3", fileUpload.SuggestedMetadataModel.MimeType);
		}
		
		[TestMethod]
		public async Task CreateMetadata_FileExists_VerifyCalls()
		{
			var id = 0;
			var hashid = new Hashids().Encode(id);
			var metadata = new MetadataModelWithAudioId()
			{
				AudioId = hashid,
				Album = "Test",
				AudioBitrate = 192,
				Duration = 1000,
				MimeType = "Test",
				Performers = "Test",
				Title = "Test"
			};
			_audioRepositoryMock
				.Setup(e => e.DoesAudioExistsAsync(id)).ReturnsAsync(true);
			await _audioService.CreateAudioMetadataAsync(metadata);
			_metadataRepositoryMock.Verify(a => a.CreateMetadataAsync(metadata, id), Times.Once);

		}
		
		[TestMethod]
		public async Task CreateMetadata_FileDoesNotExist_NoCalls()
		{
			var id = 5;
			var hashid = new Hashids().Encode(id);
			var metadata = new MetadataModelWithAudioId()
			{
				AudioId = hashid,
				Album = "Test",
				AudioBitrate = 192,
				Duration = 1000,
				MimeType = "Test",
				Performers = "Test",
				Title = "Test"
			};
			_audioRepositoryMock.Setup(e => e.DoesAudioExistsAsync(id)).ReturnsAsync(false);
			await Assert.ThrowsExceptionAsync<NotFoundException>(() => _audioService.CreateAudioMetadataAsync(metadata));
			_metadataRepositoryMock.Verify(a => a.CreateMetadataAsync(metadata, id), Times.Never);
		}
	}
}