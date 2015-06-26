using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Translator.Core
{
	public class ClangArgBuilder
	{
		public string SimMinVersion { get; set; }

		public string PathToFrameworks { get; set; }

		public string SysRoot { get; set; }

		public string ResourceDir { get; set; }

		public string PrefixHeaderFilePath { get; set; }

		public List<string> Frameworks { get; set; }

		public List<string> IncludeDirs { get; set; }

		public ClangArgBuilder ()
		{
			Frameworks = new List<string> ();
			IncludeDirs = new List<string> ();
		}

		public string[] Build ()
		{
			var clangArgs = new List<string> {
				"-v",
				"-ObjC",
				"-fmodules",  // enable modules
				"-fobjc-arc", // enable ARC
			};

			if (!string.IsNullOrWhiteSpace (SimMinVersion))
				clangArgs.Add (string.Format ("-mios-simulator-version-min={0}", SimMinVersion));

			if (!string.IsNullOrWhiteSpace (PathToFrameworks))
				clangArgs.Add (string.Format ("-F{0}", PathToFrameworks));

			if (!string.IsNullOrWhiteSpace (SysRoot)) {
				clangArgs.Add ("-isysroot");
				clangArgs.Add (SysRoot);
			}

			if (!string.IsNullOrWhiteSpace (ResourceDir)) {
				clangArgs.Add ("-resource-dir");
				clangArgs.Add (ResourceDir);
			}

			foreach (var f in Frameworks) {
				clangArgs.Add ("-framework");
				clangArgs.Add (f);
			}

			foreach (var include in IncludeDirs) {
				clangArgs.Add (string.Format ("-I{0}", include));
			}

			if (!string.IsNullOrWhiteSpace (PrefixHeaderFilePath)) {
				clangArgs.Add ("-include");
				clangArgs.Add (PrefixHeaderFilePath);
			}

			return clangArgs.ToArray ();
		}

		public override string ToString ()
		{
			return string.Join (" ", Build ());
		}
	}
}