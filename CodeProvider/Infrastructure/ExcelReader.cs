using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ExcelDataReader;
using Newtonsoft.Json;

namespace CodeProvider.Infrastructure
{
	public static class ExcelReader
	{
		public static string GetCodes(string fileName, int sheetNumber, int firstRow, int codeColumn, int descriptionColumn)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			var codes = new List<CodeDescription>();
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
						codes.Add(new CodeDescription { Code = code, Description = description });
					}
				}
			}

			return JsonConvert.SerializeObject(codes, Formatting.Indented);
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
}