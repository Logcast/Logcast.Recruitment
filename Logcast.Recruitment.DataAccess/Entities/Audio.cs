using System.ComponentModel.DataAnnotations;

namespace Logcast.Recruitment.DataAccess.Entities
{
	public class Audio
	{
		[Key] public int Id { get; set; }
		public string FileName { get; set; }
		public string Path { get; set; }
	}
}