using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Factories;
using Logcast.Recruitment.Shared.Exceptions;
using Logcast.Recruitment.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Metadata = Logcast.Recruitment.DataAccess.Entities.Metadata;

namespace Logcast.Recruitment.DataAccess.Repositories
{
	public interface IMetadataRepository
	{
		Task<Metadata> CreateMetadataAsync(MetadataModelWithAudioId metadataModelModel, int audioId);
		Task<Metadata> GetMetadataAsync(int audioId);
		Task<List<Metadata>> GetAllMetadataAsync();
	}

	public class MetadataRepository : IMetadataRepository
	{
		private readonly ApplicationDbContext _applicationDbContext;

		public MetadataRepository(IDbContextFactory dbContextFactory)
		{
			_applicationDbContext = dbContextFactory.Create();
		}
		public async Task<Metadata> CreateMetadataAsync(MetadataModelWithAudioId metadataModel, int audioId)
		{
			if (_applicationDbContext.Audios.All(x => x.Id != audioId))
			{
				throw new NotFoundException();
			}
			var newMetadata = new Metadata()
			{
				AudioId = audioId,
				MimeType = metadataModel.MimeType,  
				Album = metadataModel.Album,    
				AudioBitrate = metadataModel.AudioBitrate,
				Duration = metadataModel.Duration,
				Performers = metadataModel.Performers,
				Title = metadataModel.Title,
			};

			_applicationDbContext.Metadatas.Add(newMetadata);
			await _applicationDbContext.SaveChangesAsync();
			return newMetadata;
		}

		public async Task<Metadata> GetMetadataAsync(int audioId)
		{
			var metadata = await _applicationDbContext.Metadatas.FirstOrDefaultAsync(x => x.AudioId == audioId);
			if (metadata == null) throw new NotFoundException();
			return metadata;
		}

		public Task<List<Metadata>> GetAllMetadataAsync()
		{
			return _applicationDbContext.Metadatas.AsNoTracking().ToListAsync();
		}
	}
}