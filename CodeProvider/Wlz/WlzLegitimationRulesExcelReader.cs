using System;
using System.Collections.Generic;
using System.IO;
using ExcelDataReader;
using Newtonsoft.Json;

namespace CodeProvider.Wlz
{
	public class WlzLegitimationRulesExcelReader
	{
		public static string GetCodes(Stream stream)
		{
			var hashSet = new HashSet<string>();
			var codes = new List<WlzRule>();
			using (var reader = ExcelReaderFactory.CreateReader(stream))
			{
				GotoSheetNumber(reader, 3);
				var skip = 1;
				while (reader.Read())
				{
					if (skip-- > 0) continue;
					var declarationCode = reader.GetString(2);
					if (declarationCode == null) continue;
					var zzpCodes = reader.GetValue(4)?.ToString();
					var functionCodes = reader.GetValue(6)?.ToString();
					var start = reader.GetDateTime(7);
					var end = (DateTime?)reader.GetValue(8);
					var declarationGroup = int.Parse(reader.GetValue(9)?.ToString());
					var declarationMethod = int.Parse(reader.GetValue(10)?.ToString());
					if (end == null || end is DateTime endDate && endDate.Year >= 2017)
						if (!string.IsNullOrWhiteSpace(zzpCodes))
							foreach (var code in zzpCodes.Split(',', StringSplitOptions.RemoveEmptyEntries))
								if (hashSet.Add($"{declarationCode}{code}{start:yyMM}{end:yyMM}"))
									codes.Add(new WlzRule
									{
										DeclarationCode = declarationCode,
										ZzpId = int.Parse(code),
										Start = start,
										End = end,
										DeclarationGroup = declarationGroup,
										DeclarationMethod = declarationMethod
									});
					if (!string.IsNullOrWhiteSpace(functionCodes))
						foreach (var code in functionCodes.Split(',', StringSplitOptions.RemoveEmptyEntries))
							if (hashSet.Add($"{declarationCode}{code}{start:yyMM}{end:yyMM}"))
								codes.Add(new WlzRule
								{
									DeclarationCode = declarationCode,
									FunctionId = int.Parse(code),
									Start = start,
									End = end,
									DeclarationGroup = declarationGroup,
									DeclarationMethod = declarationMethod
								});
					if (string.IsNullOrWhiteSpace(functionCodes) && string.IsNullOrWhiteSpace(zzpCodes))
						if (hashSet.Add($"{declarationCode}{start:yyMM}{end:yyMM}"))
							codes.Add(new WlzRule
							{
								DeclarationCode = declarationCode,
								Start = start,
								End = end,
								DeclarationGroup = declarationGroup,
								DeclarationMethod = declarationMethod
							});
				}
				return JsonConvert.SerializeObject(codes, Formatting.Indented);
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

		public class WlzRule
		{
			public string DeclarationCode { get; set; }
			public int? ZzpId { get; set; }
			public DateTime Start { get; set; }
			public DateTime? End { get; set; }
			public int DeclarationMethod { get; set; }
			public int DeclarationGroup { get; set; }
			public int? FunctionId { get; set; }
		}
	}
}