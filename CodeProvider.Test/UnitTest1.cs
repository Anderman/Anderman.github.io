using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ExcelDataReader;
using Newtonsoft.Json;
using Xunit;

// ReSharper disable StringLiteralTypo

namespace CodeProvider.Test
{
	public class CodeProviderTest
	{
		[Fact]
		public void ConvertExcelToJson()
		{
			foreach (var path in Directory.GetFiles($"{WebsiteFiles.CodeProvider}"))
				DeclarationCodeProvider.Convert(path);
		}
	}

	public class DeclarationCodeProvider
	{
		public static void Convert(string path)
		{
			var fileName = Path.GetFileName(path);
			var root = Path.GetDirectoryName(path);
			//var fileName = parts[parts.Length - 1];
			var outfile = root +"/../../"+ fileName.Substring(0, 3) + ".json";
			var codes = GetCodes(path, 1, 18, 0, 1);
			var contents = JsonConvert.SerializeObject(codes, Formatting.Indented);

			File.WriteAllText(outfile, contents);
		}

		public static List<Codes> GetCodes(string fileName, int sheetNumber, int firstRow, int codeColumn, int descriptionColumn)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			var z = new List<Codes>();
			using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
			{
				using (var reader = ExcelReaderFactory.CreateReader(stream))
				{
					SelectSheet(reader, sheetNumber);
					SelectRecordDefinitionFirstRow(reader, firstRow);
					while (reader.Read())
					{
						Debug.WriteLine(reader.GetValue(0));
						var code = reader.GetValue(codeColumn)?.ToString()?.Trim();
						var description = reader.GetValue(descriptionColumn)?.ToString()?.Trim();
						if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(description))
							continue;
						z.Add(new Codes { Code = code, Description = description });
					}
				}
			}

			return z;
		}

		private static void SelectSheet(IExcelDataReader reader, int sheetNumber)
		{
			for (var i = 1; i < sheetNumber; i++)
				reader.NextResult();
		}

		private static void SelectRecordDefinitionFirstRow(IExcelDataReader reader, int rowNumber)
		{
			for (var i = 0; i < rowNumber; i++)
				reader.Read();
		}
	}

	public class Codes
	{
		public string Code;
		public string Description;
	}
}