using System;

using ClangSharp;

using Translator.Core;
using System.Linq;
using System.IO;

namespace Translator.Playground
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string[] clangArgs = new string[] {
//				"-v",
				"-ObjC",
				"-I/Applications/Xcode-beta.app/Contents/Developer/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator8.4.sdk/usr/include",
				"-F/Applications/Xcode-beta.app/Contents/Developer/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator8.4.sdk/System/Library/Frameworks/",
				"-mios-simulator-version-min=8.4",
				"-fobjc-arc",

				"-fmodules",
				"-framework", "UIKit",
				"-framework", "Foundation",
				"-isysroot", "/Applications/Xcode-beta.app/Contents/Developer/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator8.4.sdk/",
				"-resource-dir", "/Users/rzaitov/llvm-clang/build/bin/../lib/clang/3.6.2"
			};

			// iOS samples:
//			string file = "test/Cell.m";
//			string file = "/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/HomeKitCatalogCreatingHomesPairingandControllingAccessoriesandSettingUpTriggers/HMCatalog/Supporting Files/Utilities/UITableView+Updating.m";
			string file = "/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/SoZoomy/SoZoomy/ViewController.m";

			// Mac samples
//			string file = "/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/AddressBookCocoa/AddressBookCocoa.m";
//			string file = "/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/LayoutManagerDemo/LayoutManagerDemo/main.m";

			Console.WriteLine ("**Playground**");

			var pathLocator = new XamarinPathLocator ();
			string xi = pathLocator.GetAssemblyPath (Platform.iOS);
			var locator = new BindingLocator (new string[] { xi });

			var sw = new StringWriter ();
			var srcTranslator = new SourceCodeTranslator (clangArgs, locator);
			srcTranslator.Translate (file, "TestNamespace", sw);

			Console.WriteLine (sw.ToString ());

		}
	}
}
