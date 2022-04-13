using System;
using System.IO;
using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Exceptions;
using Logcast.Recruitment.DataAccess.Repositories;
using Logcast.Recruitment.Shared.Models;

namespace Logcast.Recruitment.Domain.Services
{
    public interface IAudioService
    {
        Task<Guid> StoreAudioFileAsync(byte[] file, string fileName, string contentType);
        Task StoreAudioMetadataAsync(Guid audioId, string name, string creator);
        Task<AudioModel> GetAudioMetadataAsync(Guid audioId);
        Task<AudioModel> GetAudioFileAsync(Guid audioId);
    }

    public class AudioService : IAudioService
    {
        private readonly IAudioRepository _audioRepository;
        private readonly IIdGenerator _idGenerator;

        public AudioService(IAudioRepository audioRepository, IIdGenerator idGenerator)
        {
            _audioRepository = audioRepository;
            _idGenerator = idGenerator;
        }

        public async Task<Guid> StoreAudioFileAsync(byte[] file, string fileName, string contentType)
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException($"'{nameof(fileName)}' cannot be null or empty.", nameof(fileName));
            if (string.IsNullOrEmpty(contentType))
                throw new ArgumentException($"'{nameof(contentType)}' cannot be null or empty.", nameof(contentType));

            if (contentType != "audio/mpeg")
                throw new UnsupportedContentTypeException();
            if (Path.GetExtension(fileName) != ".mp3")
                throw new UnsupportedFileTypeException();

            var audioId = _idGenerator.NewId();
            await _audioRepository
                .StoreAudioFileAsync(file, audioId, Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName), contentType)
                .ConfigureAwait(false);
            return audioId;
        }

        public async Task StoreAudioMetadataAsync(Guid audioId, string name, string creator)
        {
            if (string.IsNullOrEmpty(creator))
                throw new ArgumentException($"'{nameof(creator)}' cannot be null or empty.", nameof(creator));

            await _audioRepository.StoreAudioMetadataAsync(audioId, name, creator).ConfigureAwait(false);
        }

        public async Task<AudioModel> GetAudioMetadataAsync(Guid audioId)
        {
            return await _audioRepository.GetAudioMetadataAsync(audioId).ConfigureAwait(false);
        }

        public async Task<AudioModel> GetAudioFileAsync(Guid audioId)
        {
            return await _audioRepository.GetAudioFileAsync(audioId).ConfigureAwait(false);
        }
    }
}
