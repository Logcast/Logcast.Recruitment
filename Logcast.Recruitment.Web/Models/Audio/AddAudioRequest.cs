using System;
using System.ComponentModel.DataAnnotations;

namespace Logcast.Recruitment.Web.Models.Audio
{
	public class AddAudioRequest
	{
		public Guid AudioId { get; set; }

		[MaxLength(200)] public string Name { get; set; }

		[Required] [MaxLength(200)] public string Creator { get; set; }
	}
}
