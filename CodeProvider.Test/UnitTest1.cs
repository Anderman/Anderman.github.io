using System.IO;
using Xunit;

// ReSharper disable StringLiteralTypo

namespace CodeProvider.Test
{
	public class CodeProviderTest
	{
		[Fact]
		public void DeclarationCodes()
		{
			foreach (var path in Directory.GetFiles($"{WebsiteFiles.CodeProvider}"))
				DeclarationCodeProvider.Convert(path);
		}
		[Fact]
		public void Municipality()
		{
			foreach (var path in Directory.GetFiles($"{WebsiteFiles.Municipality}"))
				MunicipalityProvider.Convert(path);
		}
	}
}