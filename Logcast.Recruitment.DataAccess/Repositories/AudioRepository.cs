using System;
using System.IO;
using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Entities;
using Logcast.Recruitment.DataAccess.Exceptions;
using Logcast.Recruitment.DataAccess.Factories;
using Logcast.Recruitment.DataAccess.Services;
using Logcast.Recruitment.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Logcast.Recruitment.DataAccess.Repositories
{
    public interface IAudioRepository
    {
        Task StoreAudioFileAsync(byte[] file, Guid audioId, string name, string fileType, string contentType);
        Task StoreAudioMetadataAsync(Guid audioId, string name, string creator);
        Task<AudioModel> GetAudioMetadataAsync(Guid audioId);
        Task<AudioModel> GetAudioFileAsync(Guid audioId);
    }

    public class AudioRepository : IAudioRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IFileStorage _fileStorage;

        public AudioRepository(IDbContextFactory dbContextFactory, IFileStorage fileStorage)
        {
            _applicationDbContext = dbContextFactory.Create();
            _fileStorage = fileStorage;
        }

        public async Task StoreAudioFileAsync(byte[] file, Guid audioId, string name, string fileType, string contentType)
        {
            if (await _applicationDbContext.Audio.AnyAsync(x => x.Id == audioId).ConfigureAwait(false))
                throw new ArgumentException("AudioId already exists");

            var audio = new Audio(
                audioId,
                name,
                fileType,
                contentType);

            await _fileStorage.SaveAsync(audio.FileName, file).ConfigureAwait(false);

            try
            {
                _applicationDbContext.Audio.Add(audio);
                await _applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch
            {
                await _fileStorage.DeleteAsync(audio.FileName).ConfigureAwait(false);
                throw;
            }
        }

        public async Task StoreAudioMetadataAsync(Guid audioId, string name, string creator)
        {
            var audio = await _applicationDbContext.Audio.SingleOrDefaultAsync(x => x.Id == audioId).ConfigureAwait(false);
            if (audio is null)
                throw new AudioNotFoundException();

            if (!string.IsNullOrWhiteSpace(name))
                audio.Name = name;
            audio.Creator = creator;

            _applicationDbContext.Audio.Update(audio);
            await _applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<AudioModel> GetAudioMetadataAsync(Guid audioId)
        {
            var audio = await _applicationDbContext.Audio.SingleOrDefaultAsync(x => x.Id == audioId).ConfigureAwait(false);
            return audio is null
                ? throw new AudioNotFoundException()
                : audio.ToDomainModel();
        }

        public async Task<AudioModel> GetAudioFileAsync(Guid audioId)
        {
            var audio = await _applicationDbContext.Audio.SingleOrDefaultAsync(x => x.Id == audioId).ConfigureAwait(false);
            if (audio is null)
                throw new AudioNotFoundException();

            var file = await _fileStorage.GetAsync(audio.FileName).ConfigureAwait(false);
            var audioModel = audio.ToDomainModel();
            audioModel.File = file;
            return audioModel;
        }
    }
}
