using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace CodeProvider.Declaration
{
	public class CsvReader
	{
		public static string GetCodes(MemoryStream ms, int codeColumn, int descriptionColumn)
		{
			var codes = new List<CodeDescription>();
			ms.Seek(0,0);
			using (var reader = new StreamReader(ms, Encoding.Default))
			{
				reader.ReadLine();
				string line ;
				while ((line = reader.ReadLine()) != null)
				{
					var parts = line.Split(';');
					if (parts.Length < descriptionColumn) continue;
					var code = parts[codeColumn];
					var description = parts[descriptionColumn];
					codes.Add(new CodeDescription { Code = code, Description = description });
				}
			}
			return JsonConvert.SerializeObject(codes, Formatting.Indented);
		}
	}
}