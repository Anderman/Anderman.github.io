using System.Collections.Generic;
using System.IO;
using ExcelDataReader;
using Newtonsoft.Json;

// ReSharper disable StringLiteralTypo

namespace CodeProvider.Infrastructure
{
	public static class CalculationExcelReader
	{
		public static string GetCodes(Stream stream, int sheetNumber, int firstRow, int codeColumn, int unitColumn)
		{
			var codes = new List<CalculationCode>();
			using (var reader = ExcelReaderFactory.CreateReader(stream))
			{
				ExcelReader.SelectSheet(reader, sheetNumber);
				ExcelReader.SelectRecordDefinitionFirstRow(reader, firstRow);
				while (reader.Read())
				{
					var code = reader.GetValue(codeColumn)?.ToString()?.Trim();
					var unit = reader.GetValue(unitColumn)?.ToString()?.Trim();
					if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(unit))
						continue;
					codes.Add(new CalculationCode { Code = code, UnitId = GetUnitId(unit) });
				}
			}

			return JsonConvert.SerializeObject(codes, Formatting.Indented);
		}

		private static int GetUnitId(string unit)
		{
			switch (unit)
			{
				case "5 minuten": return 7;
				case "Etmaal (24 uur)": return 7;
				case "Maand": return 7;
				case "Overdag": return 11;
				case "Per cliënt": return 81;
				case "Per klant, per week": return 21;
				case "Per verrichting": return 82;
				default: return 81;
			}
		}

		public class CalculationCode
		{
			public string Code { get; set; }
			public int UnitId { get; set; }
		}
	}
}