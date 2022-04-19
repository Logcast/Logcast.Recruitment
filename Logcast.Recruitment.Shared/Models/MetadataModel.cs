namespace Logcast.Recruitment.Shared.Models
{
	public class MetadataModel
	{
		public int AudioBitrate { get; set; }
		public string MimeType { get; set; }
		public long Duration { get; set; }
		public string Title { get; set; }
		public string Album { get; set; }
		public string Performers { get; set; }
	}

	public class MetadataModelWithAudioId : MetadataModel
	{
		public string AudioId { get; set; }
	}
}