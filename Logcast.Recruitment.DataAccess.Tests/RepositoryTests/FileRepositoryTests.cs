using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Entities;
using Logcast.Recruitment.DataAccess.Exceptions;
using Logcast.Recruitment.DataAccess.Factories;
using Logcast.Recruitment.DataAccess.Repositories;
using Logcast.Recruitment.DataAccess.Tests.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Logcast.Recruitment.DataAccess.Tests.RepositoryTests
{
    [TestClass]
    public class FileRepositoryTests
    {
        private readonly IFileRepository _subscriptionRepository;
        private readonly ApplicationDbContext _testDbContext;

        public FileRepositoryTests()
        {
            var dbContextFactoryMock = new Mock<IDbContextFactory>();

            _testDbContext = EfConfig.CreateInMemoryTestDbContext();
            dbContextFactoryMock.Setup(d => d.Create()).Returns(EfConfig.CreateInMemoryApplicationDbContext());

            _subscriptionRepository = new FileRepository(dbContextFactoryMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _testDbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task SaveAudioFileAsync_ShouldAddNewFile()
        {
            await _testDbContext.Files.AddAsync(new File("name", "path"));
            await _testDbContext.SaveChangesAsync();

            var file = await _testDbContext.Files.SingleAsync();

            Assert.AreEqual("name", file.Name);
            Assert.AreEqual("path", file.Path);
        }
    }
}