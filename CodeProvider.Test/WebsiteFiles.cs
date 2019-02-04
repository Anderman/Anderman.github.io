using System;
using System.IO;

namespace CodeProvider.Test
{
	public static class WebsiteFiles
	{
		public static string CodeProvider => $"{TestRoot}/Declaration/";
		public static string Municipality => $"{TestRoot}/Municipality/";

		public static string TestRoot => Path.GetFullPath(".").ToLower().RemoveLiveTestPath()
			.Split(".test")[0]+".test/";

		public static string RemoveLiveTestPath(this string s)
		{
			var x = s.IndexOf(".vs\\", StringComparison.Ordinal);
			if (x == -1) return s;
			var y = s.IndexOf("\\t\\", StringComparison.Ordinal)+2;
			return s.Substring(0, x) + s.Substring(y);
		}
	}
}