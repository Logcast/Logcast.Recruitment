using System;
using System.IO;
using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Entities;
using Logcast.Recruitment.DataAccess.Exceptions;
using Logcast.Recruitment.DataAccess.Factories;
using Logcast.Recruitment.Shared.Models;
using Logcast.Recruitment.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Logcast.Recruitment.DataAccess.Repositories
{
    public interface IMetaDataRepository
    {
        Task<int> AddMetaDataAsync(MetaData metaData);
        Task<MetaData> GetMetaDataAsync(int metaDataId);
        Task<bool> DoesMetaDataExistAsync(int metaDataId);
    }

    public class MetaDataRepository : IMetaDataRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        

        public MetaDataRepository(IDbContextFactory dbContextFactory)
        {
            _applicationDbContext = dbContextFactory.Create();
        }

        public async Task<int> AddMetaDataAsync(MetaData metaData)
        {
            try
            {
                await _applicationDbContext.MetaData.AddAsync(metaData);
                await _applicationDbContext.SaveChangesAsync();
                return metaData.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable add metadata {metaData}: {e}");
                throw;
            }
        }

        public async Task<MetaData> GetMetaDataAsync(int metaDataId)
        {
            try
            {
                var metaData = await _applicationDbContext.MetaData.FirstOrDefaultAsync(x => x.Id == metaDataId);
                if (metaData is null)
                    throw new NotFoundException();
                return metaData;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Unable get metadata for id {metaDataId}: {e}");
                throw;
            }
        }

        public async Task<bool> DoesMetaDataExistAsync(int metaDataId)
		{
			return await _applicationDbContext.MetaData.AnyAsync(x => x.Id == metaDataId);
		}
    }
}