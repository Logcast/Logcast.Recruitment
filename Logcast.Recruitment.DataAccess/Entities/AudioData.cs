using System;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Logcast.Recruitment.Shared.Models;

namespace Logcast.Recruitment.DataAccess.Entities
{
    public class AudioData
    {
        public AudioData()
        {
        }

        public AudioData(string name, string path, Stream stream, int metaDataId)
        {
            Name = name;
            Path = path;
            Stream = stream;
            MetaDataId = metaDataId;
            Extension = System.IO.Path.GetExtension(name);
        }

        [Key] public int Id { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }
        public string Path { get; set; }
        [MaxLength(10)]
        public string Extension { get; set; }
        public Stream Stream { get; set; }
        public int MetaDataId { get; set; }
    }
}