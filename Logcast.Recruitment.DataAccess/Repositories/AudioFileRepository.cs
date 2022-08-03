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
using TagLib;

namespace Logcast.Recruitment.DataAccess.Repositories
{
    public interface IAudioFileRepository
    {
        Task<int> AddAudioFileAsync(Stream audioStream, string audioFileName, int metaDataId);
        Task<int> AddAudioDataAsync(AudioData audioData);
        Task<AudioData> GetAudioDataAsync(int audioFileId);
        Task<bool> DoesAudioDataExistAsync(int audioFileId);
        Task<bool> DeleteAudioDataAsync(int audioFileId);
    }

    public class AudioFileRepository : IAudioFileRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly string _baseDirectory;
        private const string FILESTORAGE = "UploadedAudioFiles";
        

        public AudioFileRepository(IDbContextFactory dbContextFactory)
        {
            _applicationDbContext = dbContextFactory.Create();
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        private string GetAudioFileDirectory()
        {
            return Path.Combine(_baseDirectory, FILESTORAGE);
        }

        private bool DoesServerDirectoryExist()
        {
            return Directory.Exists(GetAudioFileDirectory());
        }



        public async Task<int> AddAudioFileAsync(Stream audioStream, string audioFileName, int metaDataId)
        {
            try
            {
                if (!DoesServerDirectoryExist())
                    Directory.CreateDirectory(GetAudioFileDirectory());
                string serverFileName = Path.Combine(GetAudioFileDirectory(), string.Concat(Guid.NewGuid().ToString(), audioFileName));
                try
                {
                    var tfile = TagLib.File.Create(audioFileName);
                    var audioData = new AudioData(){
                        Name = audioFileName,
                        Path = serverFileName,
                        Stream = audioStream,
                        Extension = tfile.MimeType,
                        MetaDataId = metaDataId
                    };
                    await _applicationDbContext.AudioData.AddAsync(audioData);
                    await _applicationDbContext.SaveChangesAsync();
                    return audioData.Id;
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Unable to save audio file data: {e}");
                    throw new UnableToSaveAudioException();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"Unable to create audio file directory: {e}");
                throw new UnableToCreateDirectoryException();
            }
        }


        public async Task<int> AddAudioDataAsync(AudioData audioData)
        {
            await _applicationDbContext.AudioData.AddAsync(audioData);
            await _applicationDbContext.SaveChangesAsync();
            return audioData.Id;
        }

        public async Task<AudioData> GetAudioDataAsync(int audioFileId)
        {
            try
            {
                var audioData = await _applicationDbContext.AudioData.FirstOrDefaultAsync(x => x.Id == audioFileId);
                if (audioData is null)
                    throw new FileNotFoundException();
                return audioData;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Unable get audio data for id {audioFileId}: {e}");
                throw;
            }
        }

        public async Task<bool> DoesAudioDataExistAsync(int audioFileId)
		{
			return await _applicationDbContext.AudioData.AnyAsync(x => x.Id == audioFileId);
		}

        public async Task<bool> DeleteAudioDataAsync(int audioFileId)
        {
            try
            {
                var audioData = await GetAudioDataAsync(audioFileId);
                _applicationDbContext.AudioData.Remove(audioData);
                await _applicationDbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}