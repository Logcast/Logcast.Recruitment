using System;

namespace Logcast.Recruitment.Shared.Models
{
    public class AudioModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ContentType { get; set; }

        public string Creator { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public byte[] File { get; set; }
    }
}
