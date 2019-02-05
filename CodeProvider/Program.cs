using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using CodeProvider.Declaration;
using CodeProvider.Infrastructure;
using CodeProvider.Municipality;
using CodeProvider.Wlz;

// ReSharper disable StringLiteralTypo

namespace CodeProvider
{
	internal class Program
	{
		private const string Aw319 = "https://www.vektis.nl/streams/standaardisatie/standaarden/AW319-1.4";

		private static void Main(string[] args)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			var links = new List<string>();
			CreateDeclarationCodes(links);
			CreateMunicipalityCodes(links);
			CreateUzoviCodes(links);
			CreateWlzRules(links);
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
			var httpLinks = content.Split("\"").Where(x => x.StartsWith("/Bestanden")).Select(x => $"https://tog.vektis.nl{x}");
			foreach (var httpLink in httpLinks)
			{
				DeclarationCodeProvider.Convert(httpLink, links);
				CalculationCodeProvider.Convert(httpLink, links);
			}
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

		public static void CreateWlzRules(List<string> links)
		{
			var client = new HttpClient();
			var content = client.GetStringAsync(Aw319).Result;
			var aTags = Regex.Matches(content, @"(<a.*?>.*?</a>)",RegexOptions.Singleline).Select(x=>x.ToString()).ToArray();
			var httpLink = aTags.Where(x=>x.Contains("Koppeltabel", StringComparison.OrdinalIgnoreCase)).Select(x => x.Split("\"").FirstOrDefault(y => y.StartsWith("https://www.vektis.nl"))).OrderBy(x => x).Last();

			WlzCalculationRuleProvider.Convert(httpLink, links);
			WlzLegitimationRuleProvider.Convert(httpLink, links);
		}
	}
}