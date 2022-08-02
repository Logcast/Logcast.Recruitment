using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Logcast.Recruitment.DataAccess.Repositories;
using Logcast.Recruitment.DataAccess.Entities;
using Logcast.Recruitment.Shared.Models;
using Logcast.Recruitment.Shared.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Logcast.Recruitment.Domain.Services
{
    public interface IAudioService
    {
        Task<int> AddAudioFileAsync(IFormFile audioFile);
        Task<AudioData> GetAudioDataAsync(int audioId);
        Task<MetaData> GetMetaDataAsync(int metaDataId);
        Task<MetaData> GetMetaDataForAudioDataAsync(int audioId);
    }

    public class AudioService : IAudioService
    {
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IMetaDataRepository _metaDataRepository;
        

        public AudioService(IAudioFileRepository audioFileRepository, IMetaDataRepository metaDataRepository)
        {
            _audioFileRepository = audioFileRepository;
            _metaDataRepository = metaDataRepository;
        }

        private bool IsValidFileType(string input)
        {
            if (input == "")
                return false;
            try
            {
                string fileExt = System.IO.Path.GetExtension(input);
                string pattern = @"^.*\.(mp3|flac|m4a|rec)$";
                var regex = new Regex(pattern);
                return regex.IsMatch(fileExt.ToLower());
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> AddAudioFileAsync(IFormFile audioFile)
        {
            if ((audioFile is null) || !IsValidFileType(audioFile.FileName))
                throw new InvalidFileException();
            try
            {
                var audioStream = audioFile.OpenReadStream();
                var tfile = TagLib.File.Create(audioFile.FileName);
                var metaData = new MetaData(){
                    Title = tfile.Tag.Title,
                    Artist = tfile.Tag.Title,
                    Album = tfile.Tag.Album,
                    AlbumArtists = tfile.Tag.AlbumArtists.ToString(),
                    Genre = tfile.Tag.FirstGenre,
                    TrackNumber = tfile.Tag.Track,
                    Bitrate = tfile.Properties.AudioBitrate,
                    Duration = tfile.Properties.Duration
                };

                var audioId = await _audioFileRepository.AddAudioFileAsync(audioStream, audioFile.FileName, metaData.Id);
                await _metaDataRepository.AddMetaDataAsync(metaData);
                return audioId;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<AudioData> GetAudioDataAsync(int audioId)
        {
            return await _audioFileRepository.GetAudioDataAsync(audioId);
        }

        public async Task<MetaData> GetMetaDataAsync(int metaDataId)
        {
            return await _metaDataRepository.GetMetaDataAsync(metaDataId);
        }

        public async Task<MetaData> GetMetaDataForAudioDataAsync(int audioId)
        {
            var audioData = await GetAudioDataAsync(audioId);
            if (audioData.MetaDataId <= 0)
                throw new MissingMetadataException();
            return await GetMetaDataAsync(audioData.MetaDataId);
        }
    }
}