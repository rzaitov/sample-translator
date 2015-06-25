using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

using Translator.Core;

namespace Translator.Addin
{
	public static class CodeTranslator
	{
		public static void Translate (CodeTranslationConfiguration configuration)
		{
			UriBuilder uri = new UriBuilder (Assembly.GetExecutingAssembly ().GetName ().CodeBase);
			string path = Uri.UnescapeDataString (uri.Path);

			string strAppDir = Path.GetDirectoryName (path);
			string utilsPath = Path.Combine (strAppDir, "Utilities");
			string resources = Path.Combine (utilsPath, "resources");

			var clangArgs = new List<string> {
				"-ObjC",
				string.Format ("-F\"{0}\"", XCodeConfiguration.PathToFramewroks),
				string.Format ("-mios-simulator-version-min={0}", XCodeConfiguration.SdkVersion),
				"-fmodules",  // enable modules
				"-fobjc-arc", // enable ARC
				"-isysroot", XCodeConfiguration.SdkPath,
				"-resource-dir", resources
			};

			foreach (var f in configuration.Frameworks) {
				clangArgs.Add ("-framework");
				clangArgs.Add (f);
			}

			var pathLocator = new XamarinPathLocator ();
			string xi = pathLocator.GetAssemblyPath (Platform.iOS);
			var locator = new BindingLocator (new string[] { xi });

			var srcTranslator = new SourceCodeTranslator (clangArgs.ToArray (), locator);

			foreach (var file in configuration.FilePaths) {
				Console.WriteLine (file);
				string fileName = Path.GetFileNameWithoutExtension (file);
				fileName = Path.ChangeExtension (fileName, "cs");
				var dstPath = Path.Combine (configuration.ProjectPath, fileName);

				using (var textWriter = File.CreateText (dstPath)) {
					srcTranslator.Translate (file, configuration.ProjectNamespace, textWriter);
				}
			}
		}
	}
}

