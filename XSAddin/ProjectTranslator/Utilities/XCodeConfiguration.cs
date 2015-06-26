using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

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
			string result = string.Empty;
			var apps = Directory.GetDirectories ("/Applications");
			var xcodes = apps.Where (c => c.Contains ("Xcode")).ToList<string> ();

			double maximumVersion = 0;

			foreach (var xcode in xcodes) {
				var currentXcodeVersion = GetXcodeVersion (xcode);

				// TODO: currently Xcode 7 beta and higher versions of Xcode are not supported
				if (currentXcodeVersion > maximumVersion && currentXcodeVersion < 7.0) {
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

