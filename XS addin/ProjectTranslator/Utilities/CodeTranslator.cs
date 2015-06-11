using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

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

			info.FileName = "Utilities/sample-translator";
			info.Arguments = string.Format (" -ns {0} -dir \"{1}\" {2} -- -ObjC -I{3} -F\"{4}\" -mios-simulator-version-min=8.4 {5}",
				configuration.ProjectNamespace, configuration.ProjectPath,
				configuration.FilesToString (), XCodeConfiguration.PathToFramewroks,
				XCodeConfiguration.PathToIncludes, configuration.FramewroksToString ());
			info.WorkingDirectory = "Utilities";
			var p = Process.Start (info);
			p.WaitForExit ();
		}
	}
}

