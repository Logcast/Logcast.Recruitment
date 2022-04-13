using Logcast.Recruitment.Web;
using Logcast.Recruitment.Web.Models.Audio;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LogCast.Recruitment.IntegrationTests
{
    public class LogcastRecruitmentTests : IClassFixture<LogcastRecruitmentWebApplicationFactory<Program>>
    {
        private readonly LogcastRecruitmentWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public LogcastRecruitmentTests(LogcastRecruitmentWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Valid_upload_audio_file_request_returns_successful_response()
        {
            var stream = new MemoryStream(Encoding.ASCII.GetBytes("test file content"));
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");
            streamContent.Headers.Add("Content-Disposition", "form-data; name=\"audioFile\"; filename=\"test.mp3\"");
            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(streamContent, "audioFile", "test.mp3");

            var response = await _client.PostAsync("api/audio/audio-file", multipartContent)
                .ConfigureAwait(false);

            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);

            var result = await response.Content.ReadFromJsonAsync<UploadAudioFileResponse>()
                .ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.AudioId);
        }

        [Fact]
        public async Task Upload_audio_file_request_with_invalid_content_type_returns_error()
        {
            var stream = new MemoryStream(Encoding.ASCII.GetBytes("test file content"));
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            streamContent.Headers.Add("Content-Disposition", "form-data; name=\"audioFile\"; filename=\"test.txt\"");
            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(streamContent, "audioFile", "test.txt");

            var response = await _client.PostAsync("api/audio/audio-file", multipartContent)
                .ConfigureAwait(false);

            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.Contains("Only audio/mpeg content is supported", result);
        }
    }
}
