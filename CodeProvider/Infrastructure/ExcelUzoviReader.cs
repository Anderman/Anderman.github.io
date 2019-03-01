using System;
using System.Collections.Generic;
using System.IO;
using ExcelDataReader;
using Newtonsoft.Json;
// ReSharper disable StringLiteralTypo

namespace CodeProvider.Infrastructure
{
	public static class ExcelUzoviReader
	{
		private const int Offset = 'O' - 'A';
		private const string SheetName = "Concern";

		private static string Clean(string value)
		{
			value= value.Replace("  ", " ");
			value= value.Replace("DSW-STAD  HOLLAND", "Dsw-Stad Holland");
			value= value.Replace("DE FRIESLAND", "De Friesland");
			value= value.Replace("MULTIZORG", "Multizorg");
			value= value.Replace("ALGEMEEN", "Algemeen");
			value= value.Replace("ALG ", "Algemeen");
			value= value.Replace("T.B.V.", "t.b.v.");
			value= value.Replace("COOPERATIE", "Coöperatie");
			return value;
		}

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
						if (reader.GetString(i + Offset) == "x")
							concern = concerns[i].Replace("\n"," ").Trim();
					uzoviCodes.Add(new UzoviRecord { Concern = Clean(concern), Description = Clean(name), Code = uzoviCode });
				}
			}

			return JsonConvert.SerializeObject(uzoviCodes, Formatting.Indented);
		}

		private static string[] GetConcernNames(IExcelDataReader reader)
		{
			var concernNames = new string[reader.FieldCount - Offset];
			reader.Read();
			reader.Read();
			for (var i = Offset; i < reader.FieldCount; i++)
			{
				var name = reader.GetString(i);
				concernNames[i - Offset] = name;
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