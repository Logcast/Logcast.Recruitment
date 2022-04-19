using System;
using System.IO;
using System.Threading.Tasks;
using HashidsNet;
using Logcast.Recruitment.DataAccess.Entities;
using Logcast.Recruitment.DataAccess.Factories;
using Logcast.Recruitment.DataAccess.Repositories;
using Logcast.Recruitment.DataAccess.Tests.Configuration;
using Logcast.Recruitment.Shared.Exceptions;
using Logcast.Recruitment.Shared.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Logcast.Recruitment.DataAccess.Tests.RepositoryTests
{
	[TestClass]
	public class MetadataRepositoryTests
	{
		private readonly IMetadataRepository _metadataRepository;
		private readonly ApplicationDbContext _testDbContext;

		public MetadataRepositoryTests()
		{
			var dbContextFactoryMock = new Mock<IDbContextFactory>();

			_testDbContext = EfConfig.CreateInMemoryTestDbContext();
			dbContextFactoryMock.Setup(d => d.Create()).Returns(EfConfig.CreateInMemoryApplicationDbContext());

			_metadataRepository = new MetadataRepository(dbContextFactoryMock.Object);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_testDbContext.Database.EnsureDeleted();
		}

		[TestMethod]
		public async Task CreateMetadata_FileExists_ShouldCreateMetadata()
		{
			var audio = CreateTestAudio();
			var metadataWithFileId = CreateTestMetadataWithFileId(audio.Id);

			await _testDbContext.Audios.AddAsync(audio);
			await _testDbContext.SaveChangesAsync();

			var metadata = await _metadataRepository.CreateMetadataAsync(metadataWithFileId, audio.Id);

			AssertMetadataEquals(metadata, metadataWithFileId);
		}

		[TestMethod]
		public async Task CreateMetadata_FileDoesNotExist_ShouldThrowException()
		{
			var id = 5;
			var metadataWithFileId = CreateTestMetadataWithFileId(id);

			await Assert.ThrowsExceptionAsync<NotFoundException>(
				() => { return _metadataRepository.CreateMetadataAsync(metadataWithFileId, id); }
			);
		}

		[TestMethod]
		public async Task GetMetadata_MetadataExists_ShouldReturnMetadata()
		{
			var file = CreateTestAudio();
			var metadata = CreateTestMetadata(file.Id);
			await _testDbContext.Audios.AddAsync(file);
			await _testDbContext.Metadatas.AddAsync(metadata);
			await _testDbContext.SaveChangesAsync();

			var actual = await _metadataRepository.GetMetadataAsync(file.Id);

			AssertMetadataEquals(metadata, actual);
		}

		[TestMethod]
		public async Task GetMetadata_MetadataDoesNotExist_ShouldThrowException()
		{
			var id = 0;
			await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
				_metadataRepository.GetMetadataAsync(id));
		}

		[TestMethod]
		public async Task GetAllMetadatas_MetadataExist_ShouldReturnListOfMetadatas()
		{
			var file = CreateTestAudio();
			var metadata = CreateTestMetadata(file.Id);
			await _testDbContext.Audios.AddAsync(file);
			await _testDbContext.Metadatas.AddAsync(metadata);
			await _testDbContext.SaveChangesAsync();

			var list = await _metadataRepository.GetAllMetadataAsync();

			Assert.IsTrue(list.Count > 0);
		}

		[TestMethod]
		public async Task GetAllMetadatas_MetadataDoesNotExist_ShouldReturnEmptyList()
		{
			var list = await _metadataRepository.GetAllMetadataAsync();

			Assert.IsTrue(list.Count == 0);
		}


		private MetadataModelWithAudioId CreateTestMetadataWithFileId(int id)
		{
			var hashid = new Hashids().Encode(id);
			return new MetadataModelWithAudioId
			{
				AudioId = hashid,
				Album = "Test",
				AudioBitrate = 192,
				Duration = 1000,
				MimeType = "Test",
				Performers = "Test",
				Title = "Test"
			};
		}

		private Metadata CreateTestMetadata(int id)
		{
			var metadataModel = CreateTestMetadataWithFileId(id);
			var metadataId = new Hashids().DecodeSingle(metadataModel.AudioId);
			return new Metadata()
			{
				Id = metadataId,
				AudioId = metadataId,
				MimeType = metadataModel.MimeType,
				Album = metadataModel.Album,
				AudioBitrate = metadataModel.AudioBitrate,
				Duration = metadataModel.Duration,
				Performers = metadataModel.Performers,
				Title = metadataModel.Title,
			};
		}

		private Audio CreateTestAudio()
		{
			var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			var guid = Guid.NewGuid().ToString();
			var path = Path.Combine("uploads", guid);
			return new Audio()
			{
				Id = 1,
				Path = path,
				FileName = "text.txt"
			};
		}

		private void AssertMetadataEquals(Metadata actual, MetadataModelWithAudioId expected)
		{
			Assert.AreEqual(new Hashids().DecodeSingle(expected.AudioId), actual.Id);
			Assert.AreEqual(expected.Album, actual.Album);
			Assert.AreEqual(expected.AudioBitrate, actual.AudioBitrate);
			Assert.AreEqual(expected.Duration, actual.Duration);
			Assert.AreEqual(expected.MimeType, actual.MimeType);
			Assert.AreEqual(expected.Performers, actual.Performers);
			Assert.AreEqual(expected.Title, actual.Title);
		}

		private void AssertMetadataEquals(Metadata actual, Metadata expected)
		{
			Assert.AreEqual(expected.AudioId, actual.Id);
			Assert.AreEqual(expected.Album, actual.Album);
			Assert.AreEqual(expected.AudioBitrate, actual.AudioBitrate);
			Assert.AreEqual(expected.Duration, actual.Duration);
			Assert.AreEqual(expected.MimeType, actual.MimeType);
			Assert.AreEqual(expected.Performers, actual.Performers);
			Assert.AreEqual(expected.Title, actual.Title);
		}
	}
}