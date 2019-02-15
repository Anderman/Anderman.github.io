using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace CodeProvider
{
	public static class MyHttpClient
	{
		private static readonly HttpClientHandler NotFollowRedirect = new HttpClientHandler { AllowAutoRedirect = false };

		private static readonly HttpClientHandler CookieHandler = new HttpClientHandler { AllowAutoRedirect = true, UseCookies = true, CookieContainer = new CookieContainer() };

		public static CookieContainer Cookies
		{
			get => CookieHandler.CookieContainer;
			set => CookieHandler.CookieContainer = value;
		}

		public static string GetContent(string link)
		{
			return GetClient(new LoggingHandler(CookieHandler)).GetStringAsync(link).Result;
		}

		public static Stream GetContentAsStream(string link, out string filename)
		{
			var response = GetClient().GetAsync(link).Result;
			filename = response.Content.Headers.ContentDisposition.FileName.Trim('"');


			return response.Content.ReadAsStreamAsync().Result;
		}

		public static string GetLinkByHRef(string webPage, string hRefContains, string aContains)
		{
			var link = GetLinks(webPage, hRefContains, aContains).Last();
			return AddAuthority(webPage, link);
		}

		public static List<string> GetLinksByHRef(string webPage, string hRefContains, string aContains)
		{
			var links = GetLinks(webPage, hRefContains, aContains);
			return links.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => AddAuthority(webPage, x)).ToList();
		}

		private static string AddAuthority(string webPage, string link)
		{
			if (link.StartsWith("http", StringComparison.OrdinalIgnoreCase))
				return link;
			return new Uri(webPage).GetLeftPart(UriPartial.Authority).TrimEnd('/') + "/" + link.TrimStart('/');
		}

		private static string[] GetATags(string content)
		{
			var links = Regex.Matches(content, @"(<a.*?>.*?</a>)", RegexOptions.Singleline).Select(x => x.ToString()).ToArray();
			//Console.WriteLine(string.Join('\n',links));
			return links;
		}

		private static List<string> GetLinks(string link, string hRefContains, string aContains)
		{
			var content = GetClient().GetStringAsync(link).Result;
			var filteredATags = GetATags(content).Where(x => x.Contains(aContains, StringComparison.OrdinalIgnoreCase)).ToList();
			var filteredLinks = filteredATags.Select(x => x.Split('"').FirstOrDefault(y => y.Contains(hRefContains))).Distinct().Where(x=>!string.IsNullOrWhiteSpace(x)).ToList();
			//Console.WriteLine(string.Join('\n', filteredATags));
			//Console.WriteLine(string.Join('\n', filteredLinks));
			return filteredLinks;
		}


		private static HttpClient GetClient()
		{
			var c = new HttpClient();
			AddHeaders(c);
			return c;
		}


		private static HttpClient GetClient(HttpMessageHandler handler)
		{
			var c = new HttpClient(handler);
			AddHeaders(c);
			return c;
		}

		private static void AddHeaders(HttpClient c)
		{
			//c.DefaultRequestHeaders.Add("Accept", "*/*");
			//c.DefaultRequestHeaders.Add("Host", "puc.overheid.nl");
			//c.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
			//c.DefaultRequestHeaders.Add("Pragma", "no-cache");
			//c.DefaultRequestHeaders.Add("Connection", "keep-alive");
			//c.DefaultRequestHeaders.Add("Accept-Encoding", "deflate, br");
			c.DefaultRequestHeaders.Add("Cookie", "overheidpod.informatietype_documenttype%2Fnza%2Fthema=[{\"identifier\":\"http://puc.overheid.nl/pod/terms/informatietype/documentatie\",\"gecombineerdehoofdlijst\":\"informatietype\",\"omschrijving\":\"Releasebestanden\"}]");
			c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");
		}
	}
}