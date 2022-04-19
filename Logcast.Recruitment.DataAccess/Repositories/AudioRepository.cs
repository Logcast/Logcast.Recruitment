using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Entities;
using Logcast.Recruitment.DataAccess.Factories;
using Logcast.Recruitment.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Logcast.Recruitment.DataAccess.Repositories
{
	public interface IAudioRepository
	{
		Task<int> CreateAudioAsync(Stream stream, string fileName, CancellationToken cancellationToken);
		Task<Stream> GetAudioAsync(int audioId);
		Task<bool> DoesAudioExistsAsync(int audioId);
	}

	public class AudioRepository : IAudioRepository
	{
		private readonly ApplicationDbContext _applicationDbContext;

		public AudioRepository(IDbContextFactory dbContextFactory)
		{
			_applicationDbContext = dbContextFactory.Create();
		}

		public async Task<int> CreateAudioAsync(Stream stream, string fileName, CancellationToken cancellationToken)
		{
			var guid = Guid.NewGuid().ToString();
			var path = Path.Combine("uploads", guid);
			try
			{
				if (!Directory.Exists("uploads"))
				{
					Directory.CreateDirectory("uploads");
				}
				using (var filestream = File.OpenWrite(path))
				{
					await stream.CopyToAsync(filestream, cancellationToken);
					await filestream.FlushAsync();
				}

				var audio = new Audio()
				{
					Path = path,
					FileName = fileName,
				};
				_applicationDbContext.Audios.Add(audio);
				await _applicationDbContext.SaveChangesAsync(cancellationToken);
				return audio.Id;
			}
			catch (Exception e)
			{
				
				if (e is TaskCanceledException)
				{
					if (File.Exists(path))
					{
						File.Delete(path);
					}
				}
				else if (e is UnauthorizedAccessException)
				{
					Console.WriteLine("No Access to the Upload folder");
				}

				throw;
			}
		}

		public async Task<Stream> GetAudioAsync(int audioId)
		{
			var audio = await _applicationDbContext.Audios.FirstOrDefaultAsync(s => s.Id == audioId);
			if (audio == null) throw new NotFoundException();
			var stream = File.OpenRead(audio.Path);
			return stream;
		}

		public async Task<bool> DoesAudioExistsAsync(int audioId)
		{
			return await _applicationDbContext.Audios.AnyAsync(x => x.Id == audioId);
		}
	}
}