using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CodeProvider.Declaration;
using CodeProvider.Infrastructure;
using CodeProvider.Municipality;
using CodeProvider.Wlz;
using CodeProvider.Wmo;

// ReSharper disable StringLiteralTypo

namespace CodeProvider
{
	internal class Program
	{
		private const string Zh308 = "https://puc.overheid.nl/nza/werkenmetdbcs/"; // declarationCode for DBC
		private const string WlzRules14 = "https://www.vektis.nl/streams/standaardisatie/standaarden/AW319-1.4";
		private const string UzoviRegister = "https://www.vektis.nl/streams/zorgverzekeraars-vinden";
		private const string VektisCodes = "https://www.vektis.nl/streams/standaardisatie/codelijsten";
		private const string WmoCodes= "https://istandaarden.nl/ibieb/wmo021-productcodetabel-wmo";
		private const string JwCodes= "https://istandaarden.nl/ibieb/jz21-productcodetabel-jeugdwet";

		private static void Main(string[] args)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			var indexLinks = new List<string>();
			CreateDbcDeclarationCodes(indexLinks);
			CreateUzoviRegisterCodes(indexLinks);
			CreateDeclarationCodes(indexLinks);
			CreateMunicipalityCodes(indexLinks);
			CreateVektisCodes(indexLinks);
			CreateWlzRules(indexLinks);
			CreateIndexHtml(indexLinks);
			CreateWmoCodes(indexLinks);
			CreateJwCodes(indexLinks);
		}


		private static void CreateIndexHtml(List<string> indexLinks)
		{
			var contents = string.Join("<br/>\n", indexLinks.Select(x => $"<a href='{x}'>{x}</a>"));
			File.WriteAllText($"{WebsiteFiles.Root}index.html", contents);
		}

		private static void CreateDbcDeclarationCodes(List<string> indexLinks)
		{
			var link = MyHttpClient.GetLinkByHRef(Zh308, "nza", "Releasebestanden medisch-specialistische zorg");
			link = MyHttpClient.GetLinkByHRef(link, "nza", "Nu geldende documenten medisch-specialistische zorg");
			link = MyHttpClient.GetLinkByHRef(link, "PUC", "zorgproducten");
			DbcDeclarationCodeProvider.Convert(link, indexLinks);
		}

		public static void CreateDeclarationCodes(List<string> indexLinks)
		{
			var httpLinks = MyHttpClient.GetLinksByHRef("https://tog.vektis.nl/WebInfo.aspx?ID=Prestatiecodelijsten", "/Bestanden", "/Bestanden");
			foreach (var httpLink in httpLinks)
			{
				DeclarationCodeProvider.Convert(httpLink, indexLinks);
				CalculationCodeProvider.Convert(httpLink, indexLinks);
			}
		}

		public static void CreateWmoCodes(List<string> indexLinks)
		{
				JwWmoCodeProvider.Convert(WmoCodes, indexLinks,"Wmo");
		}

		public static void CreateJwCodes(List<string> indexLinks)
		{
				JwWmoCodeProvider.Convert(JwCodes, indexLinks,"Jw");
		}

		public static void CreateMunicipalityCodes(List<string> indexLinks)
		{
			//https://www.cbs.nl/-/media/cbs/onze%20diensten/methoden/classificaties/overig/gemeenten%20alfabetisch%202019.xls
			//https://opendata.cbs.nl/ODataApi/OData/84378NED
			foreach (var path in Directory.GetFiles($"{WebsiteFiles.Municipality}"))
				MunicipalityProvider.Convert(path, indexLinks);
		}

		public static void CreateUzoviRegisterCodes(List<string> indexLinks)
		{
			var link = MyHttpClient.GetLinkByHRef(UzoviRegister, "UZOVI_register", "");
			UzoviProvider.Convert(link, indexLinks);
		}

		public static void CreateVektisCodes(List<string> indexLinks)
		{
			var links = MyHttpClient.GetLinksByHRef(VektisCodes, "COD", "");
			foreach (var link in links)
			{
				var excelLink = MyHttpClient.GetLinkByHRef(link, "xls", "");
				VektisCodeProvider.Convert(excelLink, indexLinks);
			}
		}

		public static void CreateWlzRules(List<string> indexLinks)
		{
			var httpLink = MyHttpClient.GetLinkByHRef(WlzRules14, "https://www.vektis.nl", "Koppeltabel");
			WlzCalculationRuleProvider.Convert(httpLink, indexLinks);
			WlzLegitimationRuleProvider.Convert(httpLink, indexLinks);
		}

		public static void AgbCodes()
		{
			var c = new HttpClient(new HttpClientHandler
			{
				ClientCertificateOptions = ClientCertificateOption.Automatic
			});
			const string token = "Vektis 6f902ebd-3f4e-48b5-9ccc-5e474e20d630";
			c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Vektis", token);
			var result = c.GetStringAsync("https://test-agb-api.vektis.nl/v2/vestigingen/41410515").Result;
			Console.WriteLine(result);
		}

		public static void ReadAgb()
		{
			var c = new HttpClient();
			var search = new[]
			{
				new KeyValuePair<string, string>("ZorgpartijType", "0"),
				new KeyValuePair<string, string>("Naam", ""),
				new KeyValuePair<string, string>("NaamComparer", "1"),
				new KeyValuePair<string, string>("AGBCode", "12095"),
				new KeyValuePair<string, string>("AGBCodeComparer", "0"),
				new KeyValuePair<string, string>("DatumAanvangInschrijvingComparer", "0"),
				new KeyValuePair<string, string>("DatumAanvangInschrijving", ""),
				new KeyValuePair<string, string>("DatumEindeInschrijvingComparer", "0"),
				new KeyValuePair<string, string>("DatumEindeInschrijving", ""),
				new KeyValuePair<string, string>("Postcode", ""),
				new KeyValuePair<string, string>("Plaats", ""),
				new KeyValuePair<string, string>("HrNummer", ""),
				new KeyValuePair<string, string>("Status", "0"),
				new KeyValuePair<string, string>("Zorgsoort", "00"),
				new KeyValuePair<string, string>("Kwalificaties", "")
			};
			//c.DefaultRequestHeaders.c("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");

			//CookieContainer cookies = new CookieContainer();
			//HttpClientHandler handler = new HttpClientHandler { CookieContainer = cookies };
			//var c2 = new HttpClient(handler);
			//var result1 = c2.GetAsync("https://www.agbcode.nl/Webzoeker/Zoeken").Result;
			//var responseCookies = cookies.GetCookies(new Uri("https://www.agbcode.nl")).Cast<Cookie>();
			//foreach (Cookie cookie in responseCookies)
			//	Console.WriteLine(cookie.Name + ": " + cookie.Value);
			var result2 = c.GetAsync("https://www.agbcode.nl/Webzoeker/Zoeken").Result;

			var formUrlEncodedContent = new FormUrlEncodedContent(search);

			Console.WriteLine(result2);
			var result = c.PostAsync("https://www.agbcode.nl/Webzoeker/ZoekGeavanceerd", formUrlEncodedContent).Result;
			Console.WriteLine(result);
			using (var httpClient = new HttpClient())
			{
				using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://www.agbcode.nl/Webzoeker/ZoekGeavanceerd"))
				{
					request.Headers.TryAddWithoutValidation("Cache-Control", "no-cache");
					request.Headers.TryAddWithoutValidation("Connection", "keep-alive");
					request.Headers.TryAddWithoutValidation("Pragma", "no-cache");
					request.Headers.TryAddWithoutValidation("Accept", "*/*");
					request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
					request.Headers.TryAddWithoutValidation("Accept-Language", "nl,en-US;q=0.9,en;q=0.8");
					request.Headers.TryAddWithoutValidation("Cookie",
						"ASP.NET_SessionId=omloxc5lkgcpukhi451vit1q; _ga=GA1.2.1871966212.1548538058; __RequestVerificationToken=zCS2PpSaAtGbhbJ6_0ixW3yM0wWTObKn3RORSGVJ8lqptYjuEzglkQAJ5xigWQZTF3gZQg8ENb851dTXSdpmuY2N7QiB84avHeDVlYtNRJE1; chatbotToken=JgngVhZJUrM.dAA.OQBXAFQAUwB0ADgAVQBYAHkASwBwAEgAVwBSAEgAbABJAGEAQgB6AEEANAA.1M56A2e91AE.Y-mR3mXb9_8.xOnkxC1YlRpVONSpdJPpvq1gXBfn7WlEbA5EqYDT6y8; _gid=GA1.2.1729271990.1549378527");
					request.Headers.TryAddWithoutValidation("Referer", "https://www.agbcode.nl/Webzoeker/Zoeken");
					request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");
					request.Headers.TryAddWithoutValidation("Origin", "https://www.agbcode.nl");
					request.Headers.TryAddWithoutValidation("RequestVerificationToken", "Fm01UvJGA5OIhcQr3tRC4ygmOioufjkyKdw38RR_yCHPBiZITFydabDT4U23tApeHqjLjqsM1ZrYThtNaWwLQXynPYiv3MoCT7L7B7Af3bQ1");
					request.Headers.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");

					request.Content = new StringContent("ZorgpartijType=0&Naam=&NaamComparer=1&AGBCode=12095&AGBCodeComparer=0&DatumAanvangInschrijvingComparer=0&DatumAanvangInschrijving=&DatumEindeInschrijvingComparer=0&DatumEindeInschrijving=&Postcode=&Plaats=&HrNummer=&Status=0&Zorgsoort=00&Kwalificaties=", Encoding.UTF8, "application/x-www-form-urlencoded");

					var response = httpClient.SendAsync(request).Result;
					Console.WriteLine(response);
				}
			}
		}
	}
}