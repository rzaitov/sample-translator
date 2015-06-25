using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

using Translator.Core;
using System.Linq;

namespace Translator.Addin
{
	public static class CodeTranslator
	{
		public static void Translate (CodeTranslationConfiguration configuration)
		{
			var argBuilder = new ClangArgBuilder {
				PathToFrameworks = XCodeConfiguration.PathToFramewroks,
				SimMinVersion = XCodeConfiguration.SdkVersion,
				SysRoot = XCodeConfiguration.SdkPath,
				ResourceDir = FetchResourceDir ()
			};
			argBuilder.Frameworks.AddRange (configuration.Frameworks);

			var pathLocator = new XamarinPathLocator ();
			string xi = pathLocator.GetAssemblyPath (Platform.iOS);
			var locator = new BindingLocator (new string[] { xi });

			string[] clangArgs = argBuilder.Build ();
			var srcTranslator = new SourceCodeTranslator (clangArgs, locator);

			foreach (var file in configuration.FilePaths) {
				Console.WriteLine (file);
				var dstPath = GetDestanation (file, configuration.ProjectPath);
				using (var textWriter = File.CreateText (dstPath)) {
					srcTranslator.Translate (file, configuration.ProjectNamespace, textWriter);
				}
			}
		}

		static IEnumerable<string> FetchHeaderDirs (IEnumerable<string> headerPaths)
		{
			return headerPaths.Select (hf => Path.GetDirectoryName (hf)).Distinct ();
		}

		static string GetDestanation(string srcFileName, string dstDir)
		{
			string fileName = Path.ChangeExtension (srcFileName, "cs");
			var dstPath = Path.Combine (dstDir, fileName);
			return dstPath;
		}

		static string FetchResourceDir ()
		{
			UriBuilder uri = new UriBuilder (Assembly.GetExecutingAssembly ().GetName ().CodeBase);
			string path = Uri.UnescapeDataString (uri.Path);

			string strAppDir = Path.GetDirectoryName (path);
			string utilsPath = Path.Combine (strAppDir, "Utilities");
			string resourceDir = Path.Combine (utilsPath, "resources");

			return resourceDir;
		}
	}
}