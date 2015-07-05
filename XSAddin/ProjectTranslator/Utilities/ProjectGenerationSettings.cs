using System;
using System.Collections.Generic;

using Translator.Parser;

namespace Translator.Addin
{
	public class ProjectGenerationSettings
	{
		public string PathToXcodeProject { get; set; }

		public string CsharpSolutionParentFolder { get; set; }

		public string SolutionName { get; set; }

		public Target TargetProject { get; set; }

		public bool OverwriteAppIcons { get; set; }

		public bool OveriwriteLaunchImages { get; set; }

		public List<string> HeaderSearchPaths { get; set; }
	}
}

