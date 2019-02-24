using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using CodeProvider.Infrastructure;

namespace CodeProvider.Wmo
{
	public class JwWmoCodeProvider
	{
		private const string SubDirectory = "DeclarationCodes";

		public static void Convert(string httpLink, List<string> links, string code)
		{
			Console.WriteLine($"Reading {httpLink}");

			var outfileName = code + ".json";
			var outPath = FileHelper.GetOutputPath(SubDirectory, outfileName, links);

			var client = new HttpClient();
			using (var stream = client.GetStreamAsync(httpLink).Result)
			{
				var ms = stream.AsMemoryStream();
				var contents = ExcelReader.GetCodes(ms, 2, 3, 0, 1);
				Console.WriteLine($"Export {outPath}");

				File.WriteAllText(outPath, contents);
			}
		}
	}
}