using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Translator.Addin
{
	public static class XCodeConfiguration
	{
		static readonly string sdk = "Contents/Developer/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator{0}.sdk";
		static readonly string frameworks = Path.Combine (SdkPath, "System/Library/Frameworks/");

		static string xcodePath;
		static string XcodePath {
			get {
				xcodePath = xcodePath ?? FindLatestXcodeVersion ();
				return xcodePath;
			}
		}

		public static string SdkPath {
			get {
				return Path.Combine (XcodePath, string.Format (sdk, SdkVersion));
			}
		}

		public static string PathToFramewroks {
			get {
				return Path.Combine (XcodePath, frameworks);
			}
		}

		static string sdkVersion;
		public static string SdkVersion {
			get {
				sdkVersion = sdkVersion ?? FindActualVersionOfSDK ();
				return sdkVersion;
			}
		}

		static string FindActualVersionOfSDK ()
		{
			var index = sdk.IndexOf ("iPhoneSimulator{0}.sdk");
			var pathToSdks = Path.Combine (XcodePath, sdk.Substring (0, index));
			var sdkPaths = Directory.GetDirectories (pathToSdks);

			var versions = new List<double> ();

			foreach (string sdkPath in sdkPaths) {
				var folders = sdkPath.Split ('/');
				var sdkFolder = folders [folders.Length - 1];
				var versionString = sdkFolder.Replace (".sdk", string.Empty).Replace ("iPhoneSimulator", string.Empty);
				double version;
				Double.TryParse (versionString, out version);
				versions.Add (version);
			}

			return versions.Max ().ToString ("#.0");
		}

		static string FindLatestXcodeVersion ()
		{
			var apps = Directory.GetDirectories ("/Applications");
			var xcodes = apps.Where (c => c.Contains ("Xcode")).ToList<string> ();

			var versionsMap = xcodes.Select (p => new Tuple<string, Tuple<int, int>> (p, GetXcodeVersion (p)));
			var max = new Tuple<string, Tuple<int, int>> (string.Empty, new Tuple<int, int> (0, 0));

			foreach (var item in versionsMap) {

				if (item.Item2.Item1 >= 7)
					continue;

				if (IsGreater (item.Item2, max.Item2))
					max = item;
			}

			return max.Item1;
		}

		static bool IsGreater (Tuple <int, int> version1, Tuple <int, int> version2)
		{
			if (version1.Item1 > version2.Item1)
				return true;

			if (version1.Item1 == version2.Item1)
				return version1.Item2 > version2.Item2;

			return false;
		}

		static Tuple<int, int> GetXcodeVersion (string pathToXcode)
		{
			var pathToVersionsPlist = Path.Combine (pathToXcode, "Contents/version.plist");
			var versionPlist = XDocument.Parse (File.ReadAllText (pathToVersionsPlist));
			var versionKey = versionPlist.Descendants ().Where (p => p.Value == "CFBundleShortVersionString").First ();
			var versionValue = versionKey.ElementsAfterSelf ("string").First ();
			var majorMinor = versionValue.Value.Trim ().Split ('.');
			return new Tuple<int, int> (Int32.Parse (majorMinor [0]), Int32.Parse (majorMinor [1]));
		}
	}
}

