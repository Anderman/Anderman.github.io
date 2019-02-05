using System.IO;

namespace CodeProvider.Infrastructure
{
	public static class StringExtensions
	{
		public static string CreateOutputDirectory(this string path, string subDirectory)
		{
			var root = Path.GetDirectoryName(path);
			var directory = root + $"/../../../{subDirectory}/";
			Directory.CreateDirectory(directory);
			return directory;
		}
	}
}