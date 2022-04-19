using System.ComponentModel.DataAnnotations;

namespace Logcast.Recruitment.DataAccess.Entities
{
	public class Metadata
	{
		[Key]
		public int Id { get; set; }
		public int AudioId { get; set; }
		public int AudioBitrate { get; set; }
		public string MimeType { get; set; }
		public long Duration { get; set; }
		public string Title { get; set; }
		public string Album { get; set; }
		public string Performers { get; set; }
	}
}