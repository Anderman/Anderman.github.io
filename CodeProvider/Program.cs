using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Net.Http;
using System.Text;
using CodeProvider.Declaration;
using CodeProvider.Infrastructure;
using CodeProvider.Municipality;
// ReSharper disable StringLiteralTypo

namespace CodeProvider
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			var links = new List<string>();
			CreateDeclarationCodes(links);
			CreateMunicipalityCodes(links);
			CreateUzoviCodes(links);
			CreateIndexHtml(links);
		}

		private static void CreateIndexHtml(List<string> links)
		{
			var contents = string.Join("<br/>\n", links.Select(x => $"<a href='{x}'>{x}</a>"));
			File.WriteAllText($"{WebsiteFiles.Root}index.html", contents);
		}

		public static void CreateDeclarationCodes(List<string> links)
		{
			var client = new HttpClient();
			var content = client.GetStringAsync("https://tog.vektis.nl/WebInfo.aspx?ID=Prestatiecodelijsten").Result;
			var httpLinks = content.Split("\"").Where(x => x.StartsWith("/Bestanden")).Select(x=> $"https://tog.vektis.nl{x}");
			foreach (var httpLink in httpLinks)
				DeclarationCodeProvider.Convert(httpLink, links);
		}

		public static void CreateMunicipalityCodes(List<string> links)
		{
			//https://www.cbs.nl/-/media/cbs/onze%20diensten/methoden/classificaties/overig/gemeenten%20alfabetisch%202019.xls
			//https://opendata.cbs.nl/ODataApi/OData/84378NED
			foreach (var path in Directory.GetFiles($"{WebsiteFiles.Municipality}"))
				MunicipalityProvider.Convert(path, links);
		}

		public static void CreateUzoviCodes(List<string> links)
		{
			var link = "https://www.vektis.nl/uploads/Docs%20per%20pagina/zv%20vinden/20181116_%20UZOVI_register.xlsx";
			UzoviProvider.Convert(link, links);
		}
	}
}