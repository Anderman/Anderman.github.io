using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using CodeProvider.Infrastructure;

namespace CodeProvider.Wlz
{
	public class WlzLegitimationRuleProvider
	{
		private const string SubDirectory = "DeclarationCodes";
		private const string OutfileName = "055LegitimationRules.json";

		public static void Convert(string httpLink, List<string> links)
		{
			Console.WriteLine($"Reading {httpLink}");

			var outPath = FileHelper.GetOutputPath(SubDirectory, OutfileName, links);

			var client = new HttpClient();
			using (var stream = client.GetStreamAsync(httpLink).Result)
			{
				var contents = WlzLegitimationRulesExcelReader.GetCodes(stream.AsMemoryStream());
				Console.WriteLine($"Export {outPath}");

				File.WriteAllText(outPath, contents);
			}
		}
	}
}