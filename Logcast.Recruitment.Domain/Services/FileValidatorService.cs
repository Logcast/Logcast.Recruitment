using System;
using System.Collections.Generic;
using System.Linq;

namespace Logcast.Recruitment.Domain.Services
{
	public interface IFileValidatorService
	{
		bool CheckType(string type, byte[] data);
		int MaxSize();
	}

	public class FileValidatorService : IFileValidatorService
	{
		private class Signature
		{
			public string type;
			public byte[] value;

			public Signature(string type, byte[] value)
			{
				this.type = type.ToLower();
				this.value = value;
			}
		}

		private static List<Signature> signatures = new()
		{
			new(".mp3", new byte[] { 0xFF, 0xFB }),
			new(".mp3", new byte[] { 0xFF, 0xF3 }),
			new(".mp3", new byte[] { 0xFF, 0xF2 }),
			new(".mp3", new byte[] { 0x49, 0x44, 0x33 })
		};

		public int MaxSize()
		{
			return signatures.Max(x => x.value.Length);
		}

		public bool CheckType(string type, byte[] data)
		{
			foreach (var signature in signatures.Where(x =>
				         x.type.Equals(type, StringComparison.InvariantCultureIgnoreCase)))
			{
				if (data.Take(signature.value.Length).SequenceEqual(signature.value))
				{
					return true;
				}
			}

			return false;
		}
	}
}