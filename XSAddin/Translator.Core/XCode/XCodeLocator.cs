using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Translator.Core
{
	public class XCodeLocator
	{
		public string FindXCode (XCodeCriteria criteria)
		{
			var xcodes = FetchXCodePaths ();
			var versionsMap = xcodes.Select (p => new Tuple<string, VersionInfo> (p, ReadVersion (p)));

			var max = new Tuple<string, VersionInfo> (string.Empty, new VersionInfo());
			var comparator = new VersionComporator ();

			foreach (var item in versionsMap) {
				if (!criteria.IsSatisfy (item.Item2))
					continue;

				if (comparator.Compare(item.Item2, max.Item2) > 0)
					max = item;
			}

			return max.Item1;
		}

		IEnumerable<string> FetchXCodePaths ()
		{
			string[] apps = Directory.GetDirectories ("/Applications");
			return apps.Where (c => c.Contains ("Xcode"));
		}

		public VersionInfo ReadVersion (string pathToXcode)
		{
			string path = Path.Combine (pathToXcode, "Contents/version.plist");
			XDocument versionPlist = XDocument.Parse (File.ReadAllText (path));

			return ReadVersion (versionPlist);
		}

		public VersionInfo ReadVersion (XDocument versionPlist)
		{
			XElement xVersionKey = versionPlist.Descendants ().First (p => p.Value == "CFBundleShortVersionString");
			XElement xVersionValue = xVersionKey.ElementsAfterSelf ("string").First ();

			string[] versionInfo = xVersionValue.Value.Trim ().Split ('.');
			return new VersionInfo {
				Major = Int32.Parse (versionInfo [0]),
				Minor = Int32.Parse (versionInfo [1])
			};
		}
	}
}