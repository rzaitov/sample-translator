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

		public List<string> Frameworks { get; set; }

		public List<string> IncludeDirs { get; set; }

		public ClangArgBuilder ()
		{
			Frameworks = new List<string> ();
		}

		public string[] Build ()
		{
			if (string.IsNullOrWhiteSpace (SimMinVersion))
				throw new InvalidOperationException ();

			if (string.IsNullOrWhiteSpace (PathToFrameworks))
				throw new InvalidOperationException ();

			if (string.IsNullOrWhiteSpace(SysRoot))
				throw new InvalidOperationException ();

			if(string.IsNullOrWhiteSpace(ResourceDir))
				throw new InvalidOperationException ();

			var clangArgs = new List<string> {
				"-ObjC",
				string.Format ("-F\"{0}\"", PathToFrameworks),
				string.Format ("-mios-simulator-version-min={0}", SimMinVersion),
				"-fmodules",  // enable modules
				"-fobjc-arc", // enable ARC
				"-isysroot", SysRoot,
				"-resource-dir", ResourceDir
			};

			foreach (var f in Frameworks) {
				clangArgs.Add ("-framework");
				clangArgs.Add (f);
			}

			return clangArgs.ToArray ();
		}

		public override string ToString ()
		{
			return string.Join (" ", Build ());
		}
	}
}