using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeProvider.Declaration;
using CodeProvider.Infrastructure;
using CodeProvider.Municipality;

namespace CodeProvider
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var links = new List<string>();
			CreateDeclarationCodes(links);
			CreateMunicipalityCodes(links);
			CreateIndexHtml(links);
		}

		private static void CreateIndexHtml(List<string> links)
		{
			var contents = string.Join("<br/>", links.Select(x => $"<a src='{x}'>{x}</a>"));
			File.WriteAllText($"{WebsiteFiles.Root}index.html", contents);
		}

		public static void CreateDeclarationCodes(List<string> links)
		{
			foreach (var path in Directory.GetFiles($"{WebsiteFiles.CodeProvider}"))
				DeclarationCodeProvider.Convert(path, links);
		}

		public static void CreateMunicipalityCodes(List<string> links)
		{
			foreach (var path in Directory.GetFiles($"{WebsiteFiles.Municipality}"))
				MunicipalityProvider.Convert(path, links);
		}
	}
}