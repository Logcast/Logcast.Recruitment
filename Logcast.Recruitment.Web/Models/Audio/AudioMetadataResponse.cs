using System;

namespace Logcast.Recruitment.Web.Models.Audio
{
    public class AudioMetadataResponse
    {
        public AudioMetadataResponse(Shared.Models.AudioModel audioModel)
        {
            Creator = audioModel.Creator;
            Name = audioModel.Name;
            AudioId = audioModel.Id;
        }

        public string Creator { get; set; }
        public string Name { get; set; }
        public Guid AudioId { get; set; }
    }
}
