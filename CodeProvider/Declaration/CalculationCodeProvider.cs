using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using CodeProvider.Infrastructure;

namespace CodeProvider.Declaration
{
	public class CalculationCodeProvider
	{
		private const string SubDirectory = "DeclarationCodes";

		public static void Convert(string httpLink, List<string> links)
		{
			Console.WriteLine($"Reading {httpLink}");
			if(!httpLink.Contains("065"))
				return;
			var code = Path.GetFileName(httpLink).Substring(0, 3);
			var outfileName = code + "CalculationRules.json";

			var outPath = FileHelper.GetOutputPath(SubDirectory, outfileName, links);

			var client = new HttpClient();
			using (var stream = client.GetStreamAsync(httpLink).Result)
			{
				var ms = stream.AsMemoryStream();
				if (httpLink.EndsWith("zip"))
					ms = DeclarationCodeProvider.Unzip(ms, code);
				var contents = CalculationExcelReader.GetCodes(ms, 1, 18, 0, 3);
				Console.WriteLine($"Export {outPath}");

				File.WriteAllText(outPath, contents);
			}
		}
	}
}