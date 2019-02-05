using System;
using System.Collections.Generic;
using System.IO;
using CodeProvider.Infrastructure;

namespace CodeProvider.Municipality
{
	public class MunicipalityProvider
	{
		const string SubDirectory = "MunicipalityCodes";
		public static void Convert(string path, List<string> links)
		{
			Console.WriteLine($"Reading {path}");

			var directory = path.CreateOutputDirectory(SubDirectory);
			var outfileName = "Municipality.json";

			links.Add($"{SubDirectory}/{outfileName}");

			var contents = ExcelReader.GetCodes(path, 1, 2, 0, 2);

			var outPath = $"{directory}{outfileName}";
			Console.WriteLine($"Export {outPath}");

			File.WriteAllText(outPath, contents);
		}
	}
}