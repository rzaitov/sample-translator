using System;
using System.IO;
using System.Collections.Generic;

namespace Translator.Core
{
	public class PlatformConfiguration
	{
		const string platformsRelPath = "Contents/Developer/Platforms";
		const string sdksRelPath = "Developer/SDKs";

		public string XCodePath { get; private set; }

		public string PlatformName { get; private set; }

		public string PlatformPath {
			get {
				string platformFolderName = string.Format ("{0}.platform", PlatformName);
				return Path.Combine (XCodePath, platformsRelPath, platformFolderName);
			}
		}

		public string SdksPath {
			get {
				return Path.Combine (PlatformPath, sdksRelPath);
			}
		}

		string sdkVersion;
		public string SdkVersion {
			get {
				sdkVersion = sdkVersion ?? FindSdkVersion ();
				return sdkVersion;
			}
		}

		public string SdkPath {
			get {
				return Path.Combine (SdksPath, string.Format ("{0}{1}.sdk", PlatformName, SdkVersion));
			}
		}

		public string Frameworks {
			get {
				return Path.Combine (SdkPath, "System/Library/Frameworks");
			}
		}

		public PlatformConfiguration (string xcodePath, string platformName)
		{
			XCodePath = xcodePath;
			PlatformName = platformName;
		}

		string FindSdkVersion ()
		{
			string[] sdkPaths = Directory.GetDirectories (SdksPath);

			foreach (string sdkPath in sdkPaths) {
				string folderName = Path.GetFileName (sdkPath);
				var versionString = folderName.Replace (".sdk", string.Empty).Replace (PlatformName, string.Empty);

				double version;
				if (Double.TryParse (versionString, out version))
					return versionString;
			}
			throw new InvalidProgramException (string.Format("can determine sdk version for {0}", SdksPath));
		}
	}
}