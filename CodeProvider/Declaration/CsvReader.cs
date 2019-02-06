using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace CodeProvider.Declaration
{
	public class CsvReader
	{
		private static readonly int EndDate = (DateTime.Today.Year - 1) * 10000;
		public static string GetCodes(MemoryStream ms, int codeColumn, int descriptionColumn, int endDateColumn)
		{
			var uniqueCodes = new HashSet<string>();
			var codes = new List<CodeDescription>();
			ms.Seek(0, 0);
			using (var reader = new StreamReader(ms, Encoding.Default))
			{
				reader.ReadLine();
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					var parts = line.Split(';');
					if (parts.Length < descriptionColumn) continue;
					var code = parts[codeColumn];
					var description = parts[descriptionColumn];
					var endDate = parts[endDateColumn];
					if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(description))
						continue;
					if (!string.IsNullOrWhiteSpace(endDate) && int.Parse(endDate) < EndDate)
						continue;
					if (uniqueCodes.Add(code))
						codes.Add(new CodeDescription { Code = code, Description = description });
				}
			}
			return JsonConvert.SerializeObject(codes, Formatting.Indented);
		}
	}
}