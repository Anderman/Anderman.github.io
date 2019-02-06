using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using CodeProvider.Infrastructure;

namespace CodeProvider.Declaration
{
	public class DbcDeclarationCodeProvider
	{
		private const string SubDirectory = "DeclarationCodes";

		public static void Convert(string httpLink, List<string> links)
		{
			Console.WriteLine($"Reading {httpLink}");

			var outfileName = "Dbc.json";
			var outPath = FileHelper.GetOutputPath(SubDirectory, outfileName, links);

			var ms = MyHttpClient.GetContentAsStream(httpLink, out var fileName).AsMemoryStream();
			Console.WriteLine(httpLink);
			ms.Seek(0, 0);
			if (fileName.EndsWith("zip"))
				ms = Unzip(ms, "");
			var contents = CsvReader.GetCodes(ms, 4, 3);
			Console.WriteLine($"Export {outPath}");

			File.WriteAllText(outPath, contents);
		}

		public static MemoryStream Unzip(MemoryStream ms, string fileNameStartWith)
		{
			var archive = new ZipArchive(ms, ZipArchiveMode.Read);
			var csv = archive.Entries.Single(x => x.Name.EndsWith("csv")).Open();
			return csv.AsMemoryStream();
		}
	}
}