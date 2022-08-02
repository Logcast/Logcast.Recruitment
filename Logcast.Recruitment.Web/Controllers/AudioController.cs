using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Logcast.Recruitment.Web.Models.Audio;
using Logcast.Recruitment.Domain.Services;
using Logcast.Recruitment.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Logcast.Recruitment.Web.Controllers
{
    [ApiController]
    [Route("api/audio")]
    public class AudioController : ControllerBase
    {
        private IAudioService _audioService;

        public AudioController(IAudioService audioService)
        {
            _audioService = audioService;
        }

        [HttpPost("audio-file")]
        [SwaggerResponse(StatusCodes.Status200OK, "Audio file uploaded successfully", typeof(UploadAudioFileResponse))]
        [ProducesResponseType(typeof(UploadAudioFileResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadAudioFile(IFormFile audioFile)
        {
            try
            {
                if (audioFile is null)
                    BadRequest("Invalid file.");
                var audioDataId = await _audioService.AddAudioFileAsync(audioFile);
                return Ok();
            }
            catch(Exception e)
            {
                Console.WriteLine($"Failed to upload audio file: {e}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "Audio metadata registered successfully")]
        public async Task<IActionResult> AddAudioMetadata([Required] [FromBody] AddAudioRequest request)
        {
            return Ok();
        }

        [HttpGet("{audioId:Guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Audio metadata fetched successfully", typeof(AudioMetadataResponse))]
        [ProducesResponseType(typeof(AudioMetadataResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAudioMetadata([FromRoute] int audioId)
        {
            try
            {
                var metaData = await _audioService.GetMetaDataForAudioDataAsync(audioId);
                return Ok();
            }
            catch(Exception e)
            {
                if (e is DllNotFoundException)
                    return NotFound();
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
        }

        [HttpGet("stream/{audioId:Guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Preview stream started successfully", typeof(FileContentResult))]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAudioStream([FromRoute] int audioId)
        {
	        try
            {
                var audioData = await _audioService.GetAudioDataAsync(audioId);
                return File(audioData.Stream, audioData.Extension);
            }
            catch(Exception e)
            {
                if (e is NotFoundException)
                    return NotFound();
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
	}
}