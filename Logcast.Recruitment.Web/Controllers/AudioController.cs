using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Entities;
using Logcast.Recruitment.DataAccess.Repositories;
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
        private readonly IAudioFilesService _audioFilesService;

        public AudioController(IAudioFilesService audioFilesService)
        {
            _audioFilesService = audioFilesService;
        }

        [HttpPost("audio-file")]
        [SwaggerResponse(StatusCodes.Status200OK, "Audio file uploaded successfully", typeof(UploadAudioFileResponse))]
        [ProducesResponseType(typeof(UploadAudioFileResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadAudioFile(IFormFile audioFile, CancellationToken cancellationToken)
        {
            var audioId = await _audioFilesService.UploadAudioFile(audioFile, cancellationToken);

            return Ok(new UploadAudioFileResponse {AudioId = audioId});
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "Audio metadata registered successfully")]
        public async Task<IActionResult> AddAudioMetadata([Required] [FromBody] AddAudioRequest request)
        {
            await _audioFilesService.AddAudioMetadata(request.AudioId, request.FileName, request.Subscriber);
            
            return Ok();
        }

        [HttpGet("{audioId:Guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Audio metadata fetched successfully", typeof(AudioMetadataResponse))]
        [ProducesResponseType(typeof(AudioMetadataResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAudioMetadata([FromRoute] Guid audioId)
        {
            //TODO: Get Audio Metadata
            return Ok();
        }

        [HttpGet("stream/{audioId:Guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Preview stream started successfully", typeof(FileContentResult))]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAudioStream([FromRoute] Guid audioId, CancellationToken cancellationToken)
        {
            Stream stream = await _audioFilesService.GetAudioStreamAsync(audioId, cancellationToken);
            //TODO: Get stored audio file and return stream
            return File(stream, "audio/mpeg");
        }
    }
}