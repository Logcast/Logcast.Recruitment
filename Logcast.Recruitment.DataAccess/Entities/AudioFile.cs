using System;
using System.ComponentModel.DataAnnotations;

namespace Logcast.Recruitment.DataAccess.Entities;

public class AudioFile
{
    public Guid Id { get; }
    
    [MaxLength(260)]public string FileName { get; set; }
    [Required] [MaxLength(200)]public string ContentType { get; set; }
    [Required] public byte[] File { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    //email or id of subscriber
    public string Subscriber { get; set; }

    //Should be one to many relationship between subscriber and files, but I'm not gonna implement that
}