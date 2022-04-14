using System.ComponentModel.DataAnnotations;
using Logcast.Recruitment.Shared.Models;

namespace Logcast.Recruitment.DataAccess.Entities
{
    public class File
    {
        public File()
        {
        }

        public File(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public int Id { get; set; }

        [Required] [MaxLength(200)] public string Name { get; set; }

        [Required] public string Path { get; set; }

        [MaxLength(200)] public string Artist { get; set; }

        [MaxLength(200)] public string Album { get; set; }

        [MaxLength(200)] public string TrackTitle { get; set; }

        [MaxLength(200)] public string Genre { get; set; }

        [MaxLength(200)] public string TrackNumber { get; set; }

        public double Duration { get; set; }

        [MaxLength(100)] public string Type { get; set; }

        public long Size { get; set; }

        public int Bitrate { get; set; }

        public FileModel ToDomainModel()
        {
            return new FileModel()
            {
                Id = Id,
                Path = Path,
                Name = Name,
                Artist = Artist,
                Album = Album,
                TrackTitle = TrackTitle,
                Genre = Genre,
                TrackNumber = TrackNumber,
                Duration = Duration,
                Type = Type,
                Size = Size,
                Bitrate = Bitrate,
            };
        }
    }
}
