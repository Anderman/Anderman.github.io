using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using CodeProvider.Infrastructure;

namespace CodeProvider
{
	public class UzoviProvider

	{
		const string SubDirectory = "UzoviCodes";
		public static void Convert(string link, List<string> links)
		{
			Console.WriteLine($"Reading {link}");

			var client = new HttpClient();

			var directory = $"{WebsiteFiles.Root}{SubDirectory}/";
			Directory.CreateDirectory(directory); 
			var outfileName = "UzoviCodes.json";

			links.Add($"{SubDirectory}/{outfileName}");

			using (var stream = client.GetStreamAsync(link).Result)
			{
				var ms= new MemoryStream();
				stream.CopyTo(ms);
				var contents = ExcelUzoviReader.GetCodes(ms);
				var outPath = $"{directory}{outfileName}";
				Console.WriteLine($"Export {outPath}");

				File.WriteAllText(outPath, contents);
			}
		}
	}
}