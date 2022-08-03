using System;
using System.ComponentModel.DataAnnotations;

namespace Logcast.Recruitment.Web.Models.Audio
{
	public class AddAudioRequest
	{
		[Required]
		public int AudioDataId { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string AlbumArtists { get; set; }
        public string Genre { get; set; }
        public int Bitrate { get; set; }
        public TimeSpan Duration { get; set; }
        public uint TrackNumber { get; set; }
	}
}
