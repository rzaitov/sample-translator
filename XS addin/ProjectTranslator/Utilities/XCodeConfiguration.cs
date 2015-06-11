using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ProjectTranslator
{
	public static class XCodeConfiguration
	{
		static readonly string sdk = "Contents/Developer/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator8.4.sdk";
		static readonly string includes = Path.Combine(sdk, "System/Library/Frameworks/");
		static readonly string frameworks = Path.Combine(sdk, "/usr/include");

		static string xcodePath;
		static string XcodePath {
			get {
				xcodePath = xcodePath ?? FindLatestXcodeVersion ();
				return xcodePath;
			}
		}

		public static string SdkPath {
			get {
				return Path.Combine (XcodePath, sdk);
			}
		}

		public static string PathToIncludes {
			get {
				return Path.Combine (XcodePath, includes);
			}
		}

		public static string PathToFramewroks {
			get {
				return Path.Combine (XcodePath, frameworks);
			}
		}

		static string FindLatestXcodeVersion ()
		{
			string result = string.Empty;
			var apps = Directory.GetDirectories ("/Applications");
			var xcodes = apps.Where (c => c.Contains ("Xcode")).ToList<string> ();

			double maximumVersion = 0;

			foreach (var xcode in xcodes) {
				var currentXcodeVersion = GetXcodeVersion (xcode);
				if (currentXcodeVersion > maximumVersion) {
					maximumVersion = currentXcodeVersion;
					result = xcode;
				}
			}

			return result;
		}

		static double GetXcodeVersion (string pathToXcode)
		{
			try {
				var pathToVersionsPlist = Path.Combine (pathToXcode, "Contents/version.plist");
				var versionPlist = XDocument.Parse (File.ReadAllText (pathToVersionsPlist));
				var versionKey = versionPlist.Descendants ().Where (p => p.Value == "CFBundleShortVersionString").First ();
				var varsionValue = versionKey.ElementsAfterSelf ("string").First ();
				return Double.Parse (varsionValue.Value);
			} catch {
				return 0;
			}
		}
	}
}

