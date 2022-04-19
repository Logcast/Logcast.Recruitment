using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Factories;
using Logcast.Recruitment.DataAccess.Repositories;
using Logcast.Recruitment.DataAccess.Tests.Configuration;
using Logcast.Recruitment.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Logcast.Recruitment.DataAccess.Tests.RepositoryTests
{
	[TestClass]
	public class AudioRepositoryTests
	{
		private readonly IAudioRepository _audioRepository;
		private readonly ApplicationDbContext _testDbContext;

		public AudioRepositoryTests()
		{
			var dbContextFactoryMock = new Mock<IDbContextFactory>();

			_testDbContext = EfConfig.CreateInMemoryTestDbContext();
			dbContextFactoryMock.Setup(d => d.Create()).Returns(EfConfig.CreateInMemoryApplicationDbContext());

			_audioRepository = new AudioRepository(dbContextFactoryMock.Object);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_testDbContext.Database.EnsureDeleted();
		}

		[TestMethod]
		public async Task CreateFile_ShouldCreateFile()
		{
			var content = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			var fileName = "text.txt";
			await _audioRepository.CreateAudioAsync(new MemoryStream(content), fileName, new CancellationToken());

			var fileInfo = await _testDbContext.Audios.SingleAsync();

			Assert.AreEqual(fileInfo.FileName, fileName);
			var writtenData = await File.ReadAllBytesAsync(fileInfo.Path);
			CollectionAssert.AreEquivalent(writtenData, content);
		}

		[TestMethod]
		public async Task File_Exists_Exists_ShouldReturnTrue()
		{
			var content = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			var fileName = "text.txt";
			var id = await _audioRepository.CreateAudioAsync(new MemoryStream(content), fileName,
				new CancellationToken());

			var result = await _audioRepository.DoesAudioExistsAsync(id);

			Assert.IsTrue(result);
		}

		[TestMethod]
		public async Task File_DoesNotExist_Exists_ShouldReturnFalse()
		{
			var id = 5;
			var result = await _audioRepository.DoesAudioExistsAsync(id);

			Assert.IsFalse(result);
		}
		
		[TestMethod]
		public async Task File_DoesNotExist_GetFile_ShouldThowException()
		{
			var id = 5;

			await Assert.ThrowsExceptionAsync<NotFoundException>(() => _audioRepository.GetAudioAsync(id));
		}
	}
}