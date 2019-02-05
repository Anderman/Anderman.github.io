using System.IO;

namespace CodeProvider.Infrastructure
{
	public static class StreamExtensions
	{
		public static MemoryStream AsMemoryStream(this Stream stream)
		{
			var ms = new MemoryStream();
			stream.CopyTo(ms);
			return ms;
		}
	}
}