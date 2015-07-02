using System.Collections.Generic;

namespace Translator.UI
{
	public class ConversionPreferences
	{
		public string XcodeProjectPath { get; set; }

		public string CSharpProjectFolderPath { get; set; }

		public List<string> HeaderSearchPaths { get; set; }

		public bool OverwriteAppIcons { get; set; }

		public bool OveriwriteLaunchImages { get; set; }
	}
}

