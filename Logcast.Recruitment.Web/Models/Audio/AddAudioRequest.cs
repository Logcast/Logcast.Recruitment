using System;

namespace Logcast.Recruitment.Web.Models.Audio
{
	public class AddAudioRequest
	{
		public Guid AudioId { get; set; }
		public string FileName { get; set; }
		public string Subscriber { get; set; }
	}
}
