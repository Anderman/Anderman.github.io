using System.Collections.Generic;
using System.IO;

namespace CodeProvider.Infrastructure
{
	public static class FileHelper
	{
		public static string GetOutputPath(string subDirectory, string outfileName, List<string> links)
		{
			var directory = $"{WebsiteFiles.Root}{subDirectory}/";
			Directory.CreateDirectory(directory);

			links.Add($"{subDirectory}/{outfileName}");
			return $"{directory}{outfileName}";
		}
	}
}