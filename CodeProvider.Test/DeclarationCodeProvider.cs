using System.IO;
using Newtonsoft.Json;

namespace CodeProvider.Test
{
	public class DeclarationCodeProvider
	{
		public static void Convert(string path)
		{
			var fileName = Path.GetFileName(path);
			var root = Path.GetDirectoryName(path);
			//var fileName = parts[parts.Length - 1];
			var pathName = root + "/../../DeclarationCode/";
			Directory.CreateDirectory(pathName);
			var outfile = pathName + fileName.Substring(0, 3) + ".json";
			var codes = ExcelReader.GetCodes(path, 1, 18, 0, 1);
			var contents = JsonConvert.SerializeObject(codes, Formatting.Indented);

			File.WriteAllText(outfile, contents);
		}
	}
	public class MunicipalityProvider
	{
		public static void Convert(string path)
		{
			var fileName = Path.GetFileName(path);
			var root = Path.GetDirectoryName(path);
			//var fileName = parts[parts.Length - 1];
			var pathName = root + "/../../Municipality/";
			Directory.CreateDirectory(pathName);
			var outfile = pathName + "Municipality" + ".json";
			var codes = ExcelReader.GetCodes(path, 1, 2, 0, 2);
			var contents = JsonConvert.SerializeObject(codes, Formatting.Indented);

			File.WriteAllText(outfile, contents);
		}
	}
}