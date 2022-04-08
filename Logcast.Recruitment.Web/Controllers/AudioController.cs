using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Logcast.Recruitment.Domain.Services;
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
        private readonly IFileService _fileService;

        public AudioController(IFileService fileService)
        {
            _fileService = fileService;
        }
        
        [HttpPost("audio-file")]
        [SwaggerResponse(StatusCodes.Status200OK, "Audio file uploaded successfully", typeof(UploadAudioFileResponse))]
        [ProducesResponseType(typeof(UploadAudioFileResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadAudioFile(IFormFile audioFile)
        {
            if (audioFile == null)
            {
                return BadRequest("No file in the request.");
            }
            else if (string.IsNullOrWhiteSpace(audioFile.FileName))
            {
                return BadRequest("File name is invalid.");
            }

            try
            {
                var file = await _fileService.AddFileAsync(audioFile);

                var updatedFileWithMetadataDetails = await _fileService.ExtractMetadata(file);

                return Ok(new UploadAudioFileResponse(updatedFileWithMetadataDetails));
            }
            catch (Exception e)
            {
                if (e is UnauthorizedAccessException) return Unauthorized($"You do not have permissions to access the server folder");
                if (e is ArgumentException) return Conflict($"Invalid path or file were provided");
                if (e is ArgumentNullException) return Conflict($"Invalid path specified");
                if (e is PathTooLongException) return Conflict($"The specified path, file name, or both exceed the system-defined maximum length");                
                if (e is DirectoryNotFoundException) return Conflict($"The specified path is invalid");
                if (e is IOException) return StatusCode(409, new
                {
                    error = e.Message
                });

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "Audio metadata registered successfully")]
        public async Task<IActionResult> AddAudioMetadata([Required] [FromBody] AudioMetadataRequest request)
        {
            await _fileService.UpdateMetadata(request.Id, request.Artist, request.Album, request.TrackTitle, request.Genre, request.TrackNumber);

            return Ok();
        }

        [HttpGet("stream/{audioId:int}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Preview stream started successfully", typeof(FileContentResult))]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAudioStream([FromRoute] int audioId)
        {
            //TODO: Get stored audio file and return stream
            try
            {
                var file = await _fileService.GetFileAsync(audioId);
                var stream = await _fileService.GetAudioStream(audioId);
                return File(stream, "audio/"+file.Type);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id:int}")]
        [SwaggerResponse(StatusCodes.Status200OK, "File fetched successfully", typeof(FileModel))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "File not found")]
        [ProducesResponseType(typeof(FileModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFile([Required][FromRoute] int id)
        {
            try
            {
                var file = await _fileService.GetFileAsync(id);
                return Ok(file);
            }
            catch (Exception e)
            {
                if (e is FileNotFoundException) return NotFound("No file found with that id");
                Console.WriteLine(e);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "File fetched successfully", typeof(FileModel))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "File not found")]
        [ProducesResponseType(typeof(FileModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFiles()
        {
            var files = await _fileService.GetFilesAsync();
            return Ok(files);
        }
    }
}
