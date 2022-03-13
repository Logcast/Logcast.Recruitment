using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Entities;
using Logcast.Recruitment.DataAccess.Repositories;
using Microsoft.AspNetCore.Http;

namespace Logcast.Recruitment.Domain.Services;

public interface IAudioFilesService
{
    Task<Guid> UploadAudioFile(IFormFile file, CancellationToken cancellationToken);
    Task AddAudioMetadata(Guid audioId, string fileName, string subscriber);
    Task<Stream> GetAudioStreamAsync(Guid audioId, CancellationToken cancellationToken);
}

public class AudioFilesService : IAudioFilesService
{
    private readonly IAudioFilesRepository _audioFilesRepository;

    public AudioFilesService(IAudioFilesRepository audioFilesRepository)
    {
        _audioFilesRepository = audioFilesRepository;
    }

    public async Task<Guid> UploadAudioFile(IFormFile file, CancellationToken cancellationToken)
    {
        byte[] bytes;
        await using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms, cancellationToken);
            bytes = ms.ToArray();
        }

        var audioFile = new AudioFile {File = bytes, ContentType = file.ContentType};
        
        return await _audioFilesRepository.UploadFile(audioFile, cancellationToken);
    }

    public Task AddAudioMetadata(Guid audioId, string fileName, string subscriber)
    {
        return _audioFilesRepository.AddAudioMetadata(audioId, fileName, subscriber);
    }

    public Task<Stream> GetAudioStreamAsync(Guid audioId, CancellationToken cancellationToken)
    {
        return _audioFilesRepository.GetAudioStreamAsync(audioId, cancellationToken);
    }
}