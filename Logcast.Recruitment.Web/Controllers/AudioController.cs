using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Exceptions;
using Logcast.Recruitment.Domain.Services;
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
        public async Task<IActionResult> UploadAudioFile(IFormFile audioFile)
        {
            if (audioFile is null || audioFile.Length == 0 || string.IsNullOrWhiteSpace(audioFile.Name))
                return BadRequest(new { message = "Invalid file" });

            try
            {
                using var stream = new MemoryStream(audioFile.Length > int.MaxValue ? int.MaxValue : (int)audioFile.Length);
                await audioFile.CopyToAsync(stream);
                var audioId = await _audioService.StoreAudioFileAsync(stream.ToArray(), audioFile.FileName, audioFile.ContentType);
                return Ok(new UploadAudioFileResponse(audioId));
            }
            catch (Exception e)
            {
                if (e is UnsupportedContentTypeException)
                    return BadRequest(new { message = "Only audio/mpeg content is supported" });
                if (e is UnsupportedFileTypeException)
                    return BadRequest(new { message = "Only mp3 files are supported" });

                Console.WriteLine(e);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "Audio metadata registered successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Audio entry not found")]
        public async Task<IActionResult> AddAudioMetadata([Required][FromBody] AddAudioRequest request)
        {
            if (request == null || request.AudioId == Guid.Empty)
                return BadRequest(new { message = $"{nameof(request.AudioId)} is required" });

            try
            {
                await _audioService.StoreAudioMetadataAsync(request.AudioId, request.Name, request.Creator);
                return Ok();
            }
            catch (Exception e)
            {
                if (e is AudioNotFoundException)
                    return NotFound($"Audio with provided Id not found");

                Console.WriteLine(e);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{audioId:Guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Audio metadata fetched successfully", typeof(AudioMetadataResponse))]
        [ProducesResponseType(typeof(AudioMetadataResponse), StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Audio entry not found")]
        public async Task<IActionResult> GetAudioMetadata([FromRoute] Guid audioId)
        {
            try
            {
                var audio = await _audioService.GetAudioMetadataAsync(audioId);
                return Ok(new AudioMetadataResponse(audio));
            }
            catch (Exception e)
            {
                if (e is AudioNotFoundException)
                    return NotFound($"Audio with provided Id not found");

                Console.WriteLine(e);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("stream/{audioId:Guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Preview stream started successfully", typeof(FileContentResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Audio entry not found")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAudioStream([FromRoute] Guid audioId)
        {
            try
            {
                var audio = await _audioService.GetAudioFileAsync(audioId);
                return File(new MemoryStream(audio.File, false), audio.ContentType);
            }
            catch (Exception e)
            {
                if (e is AudioNotFoundException)
                    return NotFound($"Audio with provided Id not found");

                Console.WriteLine(e);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
