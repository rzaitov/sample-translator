using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Reflection;

namespace ProjectTranslator
{
	public sealed class CodeTranslator
	{
		static readonly CodeTranslator translator = new CodeTranslator ();

		static CodeTranslator ()
		{
		}

		CodeTranslator ()
		{
		}

		public static CodeTranslator Default { get; } = translator;

		public void Translate (CodeTranslationConfiguration configuration)
		{
			var info = new ProcessStartInfo {
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true
			};

			UriBuilder uri = new UriBuilder(Assembly.GetExecutingAssembly ().GetName ().CodeBase);
			string path = Uri.UnescapeDataString(uri.Path);

			string strAppDir = Path.GetDirectoryName(path);
			string utilsPath = Path.Combine (strAppDir, "Utilities");
			string resources = Path.Combine (utilsPath, "resources");
			string translatorPath = Path.Combine (utilsPath, "sample-translator");

			var sb = new StringBuilder ();
			// sample-translator tool's parameters
			sb.AppendFormat (" -ns {0}", configuration.ProjectNamespace);
			sb.AppendFormat (" -dir \"{0}\"", configuration.ProjectPath);
			sb.AppendFormat (" {0}", configuration.FilesToString ()); // source files
			sb.AppendFormat (" --"); // clang parameters begins here
			sb.Append (" -ObjC");
			sb.AppendFormat (" -F\"{0}\"", XCodeConfiguration.PathToFramewroks);
			sb.Append (" -mios-simulator-version-min=8.4");
			sb.AppendFormat (" {0}", configuration.FramewroksToString ());
			sb.Append (" -fmodules -fobjc-arc");
			sb.AppendFormat (" -isysroot {0}", XCodeConfiguration.SdkPath);
			sb.AppendFormat (" -resource-dir \"{0}\"", resources);

			info.FileName = translatorPath;
			info.WorkingDirectory = utilsPath;
			info.Arguments = sb.ToString ();
			var p = Process.Start (info);
			p.WaitForExit ();
		}
	}
}

