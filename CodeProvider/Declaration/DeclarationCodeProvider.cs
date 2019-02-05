using System;
using System.Collections.Generic;
using System.IO;
using CodeProvider.Infrastructure;

namespace CodeProvider.Declaration
{
	public class DeclarationCodeProvider
	{
		const string SubDirectory = "DeclarationCodes";
		public static void Convert(string path, List<string> links)
		{
			Console.WriteLine($"Reading {path}");

			var directory = path.CreateOutputDirectory(SubDirectory);
			var outfileName = Path.GetFileName(path).Substring(0, 3) + ".json";

			links.Add($"{SubDirectory}/{outfileName}");

			var contents = ExcelReader.GetCodes(path, 1, 18, 0, 1);

			var outPath = $"{directory}{outfileName}";
			Console.WriteLine($"Export {outPath}");

			File.WriteAllText(outPath, contents);
		}
	}
}