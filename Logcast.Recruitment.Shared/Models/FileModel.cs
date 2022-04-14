using System;

namespace Logcast.Recruitment.Shared.Models
{
    public class FileModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Artist { get; set; }

        public string Album { get; set; }

        public string TrackTitle { get; set; }

        public string Genre { get; set; }

        public string TrackNumber { get; set; }

        public double Duration { get; set; }

        public string Type { get; set; }

        public long Size { get; set; }

        public int Bitrate { get; set; }        
    }
}
