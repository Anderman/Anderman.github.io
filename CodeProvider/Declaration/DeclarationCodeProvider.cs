using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using CodeProvider.Infrastructure;

namespace CodeProvider.Declaration
{
	public class DeclarationCodeProvider
	{
		private const string SubDirectory = "DeclarationCodes";

		public static void Convert(string httpLink, List<string> links)
		{
			Console.WriteLine($"Reading {httpLink}");

			var directory = $"{WebsiteFiles.Root}{SubDirectory}/";
			Directory.CreateDirectory(directory);
			var code = Path.GetFileName(httpLink).Substring(0, 3);
			var outfileName = code + ".json";

			links.Add($"{SubDirectory}/{outfileName}");

			var client = new HttpClient();
			using (var stream = client.GetStreamAsync(httpLink).Result)
			{
				var ms = new MemoryStream();
				stream.CopyTo(ms);
				if (httpLink.EndsWith("zip"))
					ms = Unzip(ms, code);
				var contents = ExcelReader.GetCodes(ms, 1, 18, 0, 1);
				var outPath = $"{directory}{outfileName}";
				Console.WriteLine($"Export {outPath}");

				File.WriteAllText(outPath, contents);
			}
		}

		public static MemoryStream Unzip(MemoryStream ms, string code)
		{
			var archive = new ZipArchive(ms, ZipArchiveMode.Read);
			var excel = archive.Entries.Single(x => x.Name.EndsWith("xlsx") && x.Name.StartsWith(code)).Open();
			var ms2 = new MemoryStream();
			excel.CopyTo(ms2);
			return ms2;
		}
	}
}