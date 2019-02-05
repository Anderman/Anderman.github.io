using System;
using System.Collections.Generic;
using System.IO;
using ExcelDataReader;
using Newtonsoft.Json;

// ReSharper disable StringLiteralTypo

namespace CodeProvider.Wlz
{
	public class WlzCalculationRulesExcelReader
	{
		public static string GetCodes(Stream stream)
		{
			var codes = new List<WlzDeclarationCode>();
			var declarationCodes = new HashSet<string>();
			using (var reader = ExcelReaderFactory.CreateReader(stream))
			{
				GotoSheetNumber(reader, 3);
				var skip = 1;
				while (reader.Read())
				{
					if (skip-- > 0) continue;
					var declarationCode = reader.GetString(2);
					if (declarationCode == null) continue;
					var description = reader.GetString(3);
					var unit = reader.GetValue(5)?.ToString();
					var periodType = int.Parse(reader.GetValue(10).ToString());
					var end = (DateTime?) reader.GetValue(8);
					if (end == null || end is DateTime endDate && endDate.Year >= 2017)
						if (declarationCodes.Add(declarationCode))

							codes.Add(new WlzDeclarationCode
							{
								DeclarationCode = declarationCode,
								UnitId = GetUnitId(unit),
								PeriodType = periodType
							});
				}
			}
			return JsonConvert.SerializeObject(codes, Formatting.Indented);

		}

		private static int GetUnitId(string unit)
		{
			switch (unit)
			{
				case "Minuut": return 1;
				case "Uur": return 4;
				case "Etmaal": return 14;
				case "Dagdeel (4 uur)": return 16;
				case "Tijdsonafhankelijk": return 81;
				default: throw new Exception($"Onbekende eenheid: '{unit}'");
			}
		}

		private static void GotoSheetNumber(IExcelDataReader reader, int sheetNumber)
		{
			var currentSheet = 1;

			while (currentSheet < reader.ResultsCount)
			{
				if (currentSheet == sheetNumber) return;
				reader.NextResult();
				currentSheet++;
			}
		}

		public class WlzDeclarationCode
		{
			public string DeclarationCode { get; set; }
			public int UnitId { get; set; }
			public int PeriodType { get; set; }
		}
	}
}