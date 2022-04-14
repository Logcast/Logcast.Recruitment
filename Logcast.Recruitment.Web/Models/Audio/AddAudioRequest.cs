using System.ComponentModel.DataAnnotations;

namespace Logcast.Recruitment.Web.Models.Audio
{
	public class AudioMetadataRequest
	{
		[Required] 
		public int Id { get; set; }
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
