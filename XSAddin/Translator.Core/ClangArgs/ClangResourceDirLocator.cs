using System;
using System.Reflection;
using System.IO;

namespace Translator.Core
{
	internal class ClangResourceDirLocator
	{
		public string FindResourceDir ()
		{
			UriBuilder uri = new UriBuilder (Assembly.GetExecutingAssembly ().GetName ().CodeBase);
			string path = Uri.UnescapeDataString (uri.Path);

			string strAppDir = Path.GetDirectoryName (path);
			return Path.Combine (strAppDir, "clang/3.6.2");
		}
	}
}