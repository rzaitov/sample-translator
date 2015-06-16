using System;
using System.IO;

using XcodeProjectParser;

namespace ProjectTranslator
{
	public class XcodeProjectLoader
	{
		public static XcodeProject LoadProject (string filePath)
		{
			if (string.IsNullOrEmpty (filePath))
				throw new ArgumentNullException ("filePath");

			string fileContent = System.IO.File.ReadAllText (filePath);
			var xcodeProject = XcodeProjectReader.Parse (fileContent);
			xcodeProject.FilePath = filePath;
			return xcodeProject;
		}
	}
}

