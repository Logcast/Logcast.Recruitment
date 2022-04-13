using System;

namespace Logcast.Recruitment.Web.Models.Audio
{
	public class UploadAudioFileResponse
	{
        public UploadAudioFileResponse(Guid audioId)
        {
            AudioId = audioId;
        }

        public Guid AudioId { get; set; }
    }
}
