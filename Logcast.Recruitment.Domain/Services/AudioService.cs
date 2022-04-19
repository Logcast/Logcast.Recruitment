using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Logcast.Recruitment.Shared.Models;
using System.Threading;
using HashidsNet;
using Logcast.Recruitment.DataAccess.Repositories;
using Logcast.Recruitment.Shared.Exceptions;
using TagLib;

namespace Logcast.Recruitment.Domain.Services
{
	public interface IAudioService
	{
		Task<FileUploadModel> CreateAudioFileAsync(Stream content, string fileName, CancellationToken cancellationToken);

		Task CreateAudioMetadataAsync(MetadataModelWithAudioId metadata);

		Task<List<MetadataModelWithAudioId>> GetAllMetadataAsync();
		Task<MetadataModel> GetAudioMetadataAsync(string audioIdHash);
		Task<StreamModel> GetAudioStreamAsync(string audioIdHash);
	}

	public class AudioService : IAudioService
	{
		private readonly IAudioRepository _audioRepository;
		private readonly IMetadataRepository _metadataRepository;
		private readonly IFileValidatorService _fileValidatorService;
		private readonly Hashids _hashidsService;


		public AudioService(IAudioRepository audioRepository, IMetadataRepository metadataRepository,
			IFileValidatorService fileValidatorService, Hashids hashidsService)
		{
			_audioRepository = audioRepository;
			_metadataRepository = metadataRepository;
			_fileValidatorService = fileValidatorService;
			_hashidsService = hashidsService;
		}

		private const string EmptyTag = "Unknown";

		private class StreamAbstraction : TagLib.File.IFileAbstraction
		{
			public StreamAbstraction(string name, Stream stream)
			{
				Name = name;
				ReadStream = stream;
				WriteStream = stream;
			}

			public string Name { get; private set; }

			public Stream ReadStream { get; private set; }

			public Stream WriteStream { get; private set; }

			public void CloseStream(Stream stream)
			{
				stream.Dispose();
			}
		}

		public async Task<FileUploadModel> CreateAudioFileAsync(Stream content, string fileName,
			CancellationToken cancellationToken)
		{
			var metadataStream = new MemoryStream();
			content.Seek(0, 0);
			await content.CopyToAsync(metadataStream, cancellationToken);
			content.Seek(0, 0);
			var size = _fileValidatorService.MaxSize();
			var data = new byte[size];
			content.Read(data, 0, size);
			content.Seek(0, 0);

			if (!_fileValidatorService.CheckType(Path.GetExtension(fileName), data))
			{
				throw new ValidationFailedException();
			}

			var virtualFile = new StreamAbstraction(fileName, metadataStream);
			MetadataModel metadata;
			try
			{
				var tfile = TagLib.File.Create(virtualFile);


				metadata = new MetadataModel
				{
					AudioBitrate = tfile.Properties.AudioBitrate,
					MimeType = tfile.MimeType.Replace("taglib", "audio"),
					Duration = (long)tfile.Properties.Duration.TotalMilliseconds,
					Title = string.IsNullOrWhiteSpace(tfile.Tag.Title) ? EmptyTag : tfile.Tag.Title,
					Album = string.IsNullOrWhiteSpace(tfile.Tag.Album) ? EmptyTag : tfile.Tag.Album,
					Performers = tfile.Tag.Performers.Length == 0 ? EmptyTag : string.Join(",", tfile.Tag.Performers)
				};
			}
			catch (CorruptFileException)
			{
				throw new ValidationFailedException();
			}
			

			var audioId = await _audioRepository.CreateAudioAsync(content, fileName, cancellationToken);
			var fileUpload = new FileUploadModel
			{
				AudioId = _hashidsService.Encode(audioId),
				SuggestedMetadataModel = metadata
			};

			return fileUpload;
		}

		public async Task CreateAudioMetadataAsync(MetadataModelWithAudioId metadata)
		{
			var audioId = _hashidsService.DecodeSingle(metadata.AudioId);
			if (!await _audioRepository.DoesAudioExistsAsync(audioId))
			{
				throw new NotFoundException();
			}

			await _metadataRepository.CreateMetadataAsync(metadata, audioId);
		}

		public async Task<List<MetadataModelWithAudioId>> GetAllMetadataAsync()
		{
			var result = new List<MetadataModelWithAudioId>();
			foreach (var metadata in await _metadataRepository.GetAllMetadataAsync())
			{
				result.Add(new MetadataModelWithAudioId()
				{
					Album = metadata.Album,
					Duration = metadata.Duration,
					Performers = metadata.Performers,
					Title = metadata.Title,
					AudioBitrate = metadata.AudioBitrate,
					MimeType = metadata.MimeType,
					AudioId = _hashidsService.Encode(metadata.AudioId)
				});
			}
			return result;
		}

		public async Task<MetadataModel> GetAudioMetadataAsync(string audioIdHash)
		{
			var audioId = _hashidsService.DecodeSingle(audioIdHash);
			var metadata = await _metadataRepository.GetMetadataAsync(audioId);
			var metadataModel = new MetadataModel()
			{
				Album = metadata.Album,
				Duration = metadata.Duration,
				Performers = metadata.Performers,
				Title = metadata.Title,
				AudioBitrate = metadata.AudioBitrate,
				MimeType = metadata.MimeType
			};
			return metadataModel;
		}

		public async Task<StreamModel> GetAudioStreamAsync(string audioIdHash)
		{
			var audioId = _hashidsService.DecodeSingle(audioIdHash);
			var metadata = await _metadataRepository.GetMetadataAsync(audioId);
			var stream = await _audioRepository.GetAudioAsync(audioId);
			return new StreamModel(){ Stream = stream, MimeType = metadata.MimeType};
		}
	}
}