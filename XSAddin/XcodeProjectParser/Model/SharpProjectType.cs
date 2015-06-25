using System;
using System.ComponentModel;

namespace Translator.Parser
{
	public enum SharpProjectType
	{
		None = 0,

		[Description ("com.apple.product-type.application")]
		iOSApplication,

		[Description ("com.apple.product-type.application.watchapp")]
		WatchApp,

		// TODO: add mac application description
		[Description ("")]
		MacApplication,

		[Description ("com.apple.product-type.watchkit-extension")]
		iOSApplicationExtension
	}

	public static partial class Extensions
	{
		static readonly string iOSAppTypeGuid = "{FEACFBD2-3405-455C-9665-78FE426C6842};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
		static readonly string iOSAppExtensionTypeGuid = "{FEACFBD2-3405-455C-9665-78FE426C6842};{EE2C853D-36AF-4FDB-B1AD-8E90477E2198};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
		static readonly string WatchAppTypeGuid = "{FEACFBD2-3405-455C-9665-78FE426C6842};{D73F8E79-B4DD-4AB0-A767-D9FA3E2FE740};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
		static readonly string MacTypeGuid = "{A3F8F2AB-B479-4A4A-A458-A89E7DC349F1};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

		public static string GetProjectTypeGuid (this SharpProjectType projectType)
		{
			switch (projectType) {
			case SharpProjectType.iOSApplication:
				return iOSAppTypeGuid;
			case SharpProjectType.WatchApp:
				return WatchAppTypeGuid;
			case SharpProjectType.iOSApplicationExtension:
				return iOSAppExtensionTypeGuid;
			case SharpProjectType.MacApplication:
				return MacTypeGuid;
			default:
				return string.Empty;
			}
		}
	}
}

