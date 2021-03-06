﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

using Translator.Core;
using System.Linq;
using System.Threading.Tasks;

namespace Translator.Addin
{
	public static class CodeTranslator
	{
		public static void Translate (CodeTranslationConfiguration configuration)
		{
			var xcodeLocator = new XCodeLocator ();
			var criteria = new XCodeCriteria {
				Max = new VersionInfo {
					Major = 7
				}
			};
			string xcodePath = xcodeLocator.FindXCode (criteria);
			var xcodeConfig = new XCodeConfiguration (xcodePath);

			var argBuilder = new ClangArgBuilder {
				PathToFrameworks = xcodeConfig.IPhoneSimulator.Frameworks,
				SimMinVersion = xcodeConfig.IPhoneSimulator.SdkVersion,
				SysRoot = xcodeConfig.IPhoneSimulator.SdkPath,
				PrefixHeaderFilePath = configuration.PCHFilePath
			};
			argBuilder.Frameworks.AddRange (configuration.Frameworks);
			argBuilder.IncludeDirs.AddRange (FetchHeaderDirs (configuration.HeaderFilePaths));
			argBuilder.IncludeDirs.AddRange (configuration.HeaderSearchPaths);

			var pathLocator = new XamarinPathLocator ();
			string xi = pathLocator.GetAssemblyPath (Platform.iOS);
			var locator = new BindingLocator (new string[] { xi });

			Console.WriteLine (argBuilder.ToString ());
			string[] clangArgs = argBuilder.Build ();
			var srcTranslator = new SourceCodeTranslator (clangArgs, locator);

			foreach (var file in configuration.SourceFilePaths) {
				Console.WriteLine (file);
				var dstPath = GetDestination (file, configuration.ProjectPath);
				using (var textWriter = File.CreateText (dstPath)) {
					var options = new TranslatorOptions {
						SkipMethodBody = false,
						Namespace = configuration.ProjectNamespace,
					};
					try {
						srcTranslator.Translate (file, options, textWriter);
					} catch (Exception ex) {
						Console.WriteLine (ex);
						options.SkipMethodBody = true;
						srcTranslator.Translate (file, options, textWriter);
					}
				}
				Console.WriteLine ("done: {0}", dstPath);
			}
		}

		public static string GetDestination(string srcFileName, string dstDir)
		{
			string fileName = Path.GetFileName (srcFileName);
			fileName = Path.ChangeExtension (fileName, "cs");
			var dstPath = Path.Combine (dstDir, fileName);
			return dstPath;
		}

		static IEnumerable<string> FetchHeaderDirs (IEnumerable<string> headerPaths)
		{
			return headerPaths.Select (hf => Path.GetDirectoryName (hf)).Distinct ();
		}
	}
}