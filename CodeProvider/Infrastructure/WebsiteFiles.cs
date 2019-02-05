using System;
using System.IO;
// ReSharper disable StringLiteralTypo

namespace CodeProvider.Infrastructure
{
	public static class WebsiteFiles
	{
		public static string CodeProvider => $"{ProjectRoot}/Declaration/Resources/";
		public static string Municipality => $"{ProjectRoot}/Municipality/Resources/";

		public static string ProjectRoot = $"{Root}codeprovider/";
		public static string Root => Path.GetFullPath(".").ToLower().RemoveLiveTestPath()
			.Split("codeprovider")[0]+ "codeprovider/";

		public static string RemoveLiveTestPath(this string s)
		{
			var x = s.IndexOf(".vs\\", StringComparison.Ordinal);
			if (x == -1) return s;
			var y = s.IndexOf("\\t\\", StringComparison.Ordinal)+2;
			return s.Substring(0, x) + s.Substring(y);
		}
	}
}