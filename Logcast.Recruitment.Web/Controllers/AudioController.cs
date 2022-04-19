using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Logcast.Recruitment.Domain.Services;
using Logcast.Recruitment.Shared.Exceptions;
using Logcast.Recruitment.Shared.Models;
using Logcast.Recruitment.Web.Models.Audio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Logcast.Recruitment.Web.Controllers
{
	[ApiController]
	[Route("api/audio")]
	public class AudioController : ControllerBase
	{
		private readonly IAudioService _audioService;

		public AudioController(IAudioService audioService)
		{
			_audioService = audioService;
		}

		[HttpPost("audio-file")]
		[SwaggerResponse(StatusCodes.Status200OK, "Audio file uploaded successfully", typeof(UploadAudioFileResponse))]
		[ProducesResponseType(typeof(UploadAudioFileResponse), StatusCodes.Status200OK)]
		public async Task<IActionResult> UploadAudioFileAsync([FromForm] IFormFile audioFile,
			CancellationToken cancellationToken)
		{
			try
			{
				var fileUpload = await _audioService.CreateAudioFileAsync(audioFile.OpenReadStream(),
					audioFile.FileName,
					cancellationToken);
				return Ok(fileUpload);
			}
			catch (Exception e)
			{
				if (e is ValidationFailedException)
				{
					return BadRequest("File validation failed");
				}
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpPost]
		[SwaggerResponse(StatusCodes.Status200OK, "Audio metadata registered successfully")]
		public async Task<IActionResult> AddAudioMetadataAsync([Required] [FromBody] MetadataModelWithAudioId request)
		{
			try
			{
				await _audioService.CreateAudioMetadataAsync(request);
				return Ok();
			}
			catch (Exception e)
			{
				if (e is NotFoundException) return NotFound();
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpGet("{audioId}")]
		[SwaggerResponse(StatusCodes.Status200OK, "Audio metadata fetched successfully", typeof(MetadataModel))]
		[ProducesResponseType(typeof(MetadataModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAudioMetadataAsync([FromRoute] string audioId)
		{
			try
			{
				var metadata = await _audioService.GetAudioMetadataAsync(audioId);
				return Ok(metadata);
			}
			catch (Exception e)
			{
				if (e is NotFoundException) return NotFound();
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpGet("stream/{audioId}")]
		[SwaggerResponse(StatusCodes.Status200OK, "Preview stream started successfully", typeof(FileContentResult))]
		[ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAudioStreamAsync([FromRoute] string audioId)
		{
			try
			{
				var streamModel = await _audioService.GetAudioStreamAsync(audioId);
				Response.Headers["Content-Length"] = streamModel.Stream.Length.ToString();
				Response.Headers["Content-Type"] = streamModel.MimeType;
				return File(streamModel.Stream, streamModel.MimeType);
			}
			catch (Exception e)
			{
				if (e is NotFoundException) return NotFound();
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpGet]
		[SwaggerResponse(StatusCodes.Status200OK, "All metadatas fetched successfully", typeof(List<MetadataModel>))]
		[ProducesResponseType(typeof(MetadataModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAllMetadataAsync()
		{
			try
			{
				var metadatas = await _audioService.GetAllMetadataAsync();
				return Ok(metadatas);
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}