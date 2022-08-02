using System;

namespace Logcast.Recruitment.Shared.Models
{
    public class AudioFileModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public byte[] Data { get; set; }
        public int MetaDataId { get; set; }
    }
}