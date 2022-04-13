using System;
using System.ComponentModel.DataAnnotations;
using Logcast.Recruitment.Shared.Models;

namespace Logcast.Recruitment.DataAccess.Entities
{
    public class Audio
    {
        public Audio()
        {
        }

        public Audio(Guid id, string name, string fileType, string contentType)
        {
            Id = id;
            Name = name;
            ContentType = contentType;
            FileType = fileType;
            CreatedDate = DateTimeOffset.Now;
        }

        public Guid Id { get; set; }

        [Required] [MaxLength(200)] public string Name { get; set; }

        [Required] [MaxLength(50)] public string ContentType { get; set; }

        [Required] [MaxLength(10)] public string FileType { get; set; }

        [MaxLength(200)] public string Creator { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public string FileName => $"{Id}{FileType}";

        public AudioModel ToDomainModel()
        {
            return new AudioModel()
            {
                ContentType = ContentType,
                CreatedDate = CreatedDate,
                Creator = Creator,
                Id = Id,
                Name = Name,
            };
        }
    }
}
