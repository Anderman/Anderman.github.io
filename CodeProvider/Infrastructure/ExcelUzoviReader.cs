using System;
using System.Collections.Generic;
using System.IO;
using ExcelDataReader;
using Newtonsoft.Json;

namespace CodeProvider.Infrastructure
{
	public static class ExcelUzoviReader
	{
		private static readonly int offset = 'O' - 'A';
		const string SheetName = "Concern";

		public static string GetCodes(Stream stream)
		{
			var uzoviCodes = new List<UzoviRecord>();
			using (var reader = ExcelReaderFactory.CreateReader(stream))
			{
				SelectSheet(reader, SheetName);
				var concerns = GetConcernNames(reader);
				while (reader.Read())
				{
					var concern = "";
					var uzoviCode = int.Parse(reader.GetString(0));
					var name = reader.GetString(1);
					for (var i = 0; i < concerns.Length; i++)
						if (reader.GetString(i + offset) == "x")
							concern = concerns[i];
					uzoviCodes.Add(new UzoviRecord { Concern = concern, Description = name, Code = uzoviCode });
				}
			}

			return JsonConvert.SerializeObject(uzoviCodes, Formatting.Indented);
		}

		private static string[] GetConcernNames(IExcelDataReader reader)
		{
			var concernNames = new string[reader.FieldCount - offset];
			reader.Read();
			reader.Read();
			for (var i = offset; i < reader.FieldCount; i++)
			{
				var name = reader.GetString(i);
				concernNames[i - offset] = name;
			}

			return concernNames;
		}

		private static void SelectSheet(IExcelDataReader reader, string sheetName)
		{
			var currentSheet = 1;

			while (reader.Name != sheetName)
			{
				reader.NextResult();
				currentSheet++;
				if (currentSheet == reader.ResultsCount) throw new Exception($"Sheet {sheetName} not found");
			}
		}
	}
}