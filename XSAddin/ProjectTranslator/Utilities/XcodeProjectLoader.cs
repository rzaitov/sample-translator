using System;
using System.IO;

using Translator.Parser;

namespace Translator.Addin
{
	public class XcodeProjectLoader
	{
		public static XcodeProject LoadProject (string filePath)
		{
			if (string.IsNullOrEmpty (filePath))
				throw new ArgumentNullException ("filePath");

			var files = Directory.GetFiles (filePath, "*.pbxproj");

			if (files.Length == 0)
				throw new ArgumentNullException ("No *.pbxproj file found in selected directory");

			string fileContent = File.ReadAllText (files [0]);
			var xcodeProject = XcodeProjectReader.Parse (fileContent);
			xcodeProject.FilePath = filePath;
			return xcodeProject;
		}
	}
}

