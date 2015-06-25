﻿using System;
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

		public ProjectGenerationSettings ()
		{
		}
	}
}

