using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using CodeProvider.Infrastructure;

namespace CodeProvider.Declaration
{
	public class VektisCodeProvider
	{
		private const string SubDirectory = "ReferenceCodes";

		public static void Convert(string httpLink, List<string> links)
		{
			Console.WriteLine($"Reading {httpLink}");

			var code = Path.GetFileName(httpLink).Substring(3, 3);
			var outfileName = code + ".json";
			var outPath = FileHelper.GetOutputPath(SubDirectory, outfileName, links);

			var client = new HttpClient();
			using (var stream = client.GetStreamAsync(httpLink).Result)
			{
				var ms = stream.AsMemoryStream();
				if (httpLink.EndsWith("zip"))
					ms = Unzip(ms, code);
				var contents = ExcelReader.GetCodes(ms, 1, 17, 0, 1);
				Console.WriteLine($"Export {outPath}");

				File.WriteAllText(outPath, contents);
			}
		}

		public static MemoryStream Unzip(MemoryStream ms, string fileNameStartWith)
		{
			var archive = new ZipArchive(ms, ZipArchiveMode.Read);
			var excel = archive.Entries.Single(x => x.Name.EndsWith("xlsx") && x.Name.StartsWith(fileNameStartWith)).Open();
			return excel.AsMemoryStream();
		}
	}
}