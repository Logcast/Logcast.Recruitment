using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Entities;
using Logcast.Recruitment.DataAccess.Factories;
using Logcast.Recruitment.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Logcast.Recruitment.DataAccess.Repositories;

public interface IAudioFilesRepository
{
    Task<Guid> UploadFile(AudioFile file, CancellationToken cancellationToken);
    Task AddAudioMetadata(Guid audioId, string fileName, string subscriber);
    Task<Stream> GetAudioStreamAsync(Guid audioId, CancellationToken cancellationToken);
}

public class AudioFilesRepository : IAudioFilesRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public AudioFilesRepository(IDbContextFactory dbContextFactory)
    {
        _applicationDbContext = dbContextFactory.Create();
    }
    
    public async Task<Guid> UploadFile(AudioFile file, CancellationToken cancellationToken)
    {
        var tracked = _applicationDbContext.AudioFiles.Add(file).Entity;

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return tracked.Id;
    }

    public async Task AddAudioMetadata(Guid audioId, string fileName, string subscriber)
    {
        var existingAudio = await _applicationDbContext.AudioFiles.FirstOrDefaultAsync(x => x.Id == audioId);

        if (existingAudio == null)
        {
            throw new NotFoundException($"Audio file with id: {audioId} not uploaded");
        }
        
        existingAudio.FileName = fileName;
        existingAudio.Subscriber = subscriber;

        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task<Stream> GetAudioStreamAsync(Guid audioId, CancellationToken cancellationToken)
    {
        var existingAudio = await _applicationDbContext.AudioFiles.FirstOrDefaultAsync(x => x.Id == audioId, cancellationToken);

        if (existingAudio == null)
        {
            throw new NotFoundException($"Audio file with id: {audioId} not uploaded");
        }

        return new MemoryStream(existingAudio.File);
    }
}