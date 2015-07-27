using System;

using ClangSharp;

using Translator.Core;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Translator.Playground
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string[] clangArgs = new string[] {
				"-v",
				"-ObjC",
//				"-I/Applications/Xcode-beta.app/Contents/Developer/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator8.4.sdk/usr/include",
				"-F/Applications/Xcode-beta.app/Contents/Developer/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator8.4.sdk/System/Library/Frameworks/",
				"-mios-simulator-version-min=8.4",
				"-fobjc-arc",

				"-fmodules",
				"-framework", "UIKit",
				"-framework", "Foundation",
				"-isysroot", "/Applications/Xcode-beta.app/Contents/Developer/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator8.4.sdk/",
				"-resource-dir", "/Users/rzaitov/llvm-clang/build/bin/../lib/clang/3.6.2",
//				"-include", "/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/LazyTableImages/LazyTable_Prefix.pch"

				"-include", "/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/AdvancedUserInterfacesUsingCollectionView/AdvancedCollectionView/AdvancedCollectionView-Prefix.pch",
				"-I/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/AdvancedUserInterfacesUsingCollectionView/AdvancedCollectionView/DataSources",
				"-I/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/AdvancedUserInterfacesUsingCollectionView/AdvancedCollectionView/Views",
				"-I/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/AdvancedUserInterfacesUsingCollectionView/AdvancedCollectionView/Layouts",
				"-I/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/AdvancedUserInterfacesUsingCollectionView/AdvancedCollectionView",
				"-I/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/AdvancedUserInterfacesUsingCollectionView/AdvancedCollectionView/Categories",
				"-I/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/AdvancedUserInterfacesUsingCollectionView/AdvancedCollectionView/DataSources",
				"-I/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/AdvancedUserInterfacesUsingCollectionView/AdvancedCollectionView/ViewControllers",

			};

			// iOS samples:
//			string file = "test/Cell.m";
//			string file = "/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/HomeKitCatalogCreatingHomesPairingandControllingAccessoriesandSettingUpTriggers/HMCatalog/Supporting Files/Utilities/UITableView+Updating.m";
//			string file = "/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/SoZoomy/SoZoomy/ViewController.m";
//			string file = "/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/SoZoomy/SoZoomy/PreviewView.m";
//			string file = "/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/SoZoomy/SoZoomy/FaceView.m";
//			string file = "/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/LazyTableImages/Classes/LazyTableAppDelegate.m";
//			string file = "/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/AVCam-iOSUsingAVFoundationtoCaptureImagesandMovies/AVCam/AAPLCameraViewController.m";
			string file = "/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/AdvancedUserInterfacesUsingCollectionView/AdvancedCollectionView/Layouts/AAPLCollectionViewGridLayout.m";

			// Mac samples
//			string file = "/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/AddressBookCocoa/AddressBookCocoa.m";
//			string file = "/Users/rzaitov/Documents/Apps/Xamarin/samples/apple-samples/LayoutManagerDemo/LayoutManagerDemo/main.m";

			Console.WriteLine ("**Playground**");

			var pathLocator = new XamarinPathLocator ();
			string xi = pathLocator.GetAssemblyPath (Platform.iOS);
			var locator = new BindingLocator (new string[] { xi });

			var sw = new StringWriter ();
			var srcTranslator = new SourceCodeTranslator (clangArgs, locator);
			var tu = srcTranslator.GetTranslationUnit (file);
			tu.Dump ();

			var options = new TranslatorOptions {
				SkipMethodBody = false,
				Namespace = "TestNamespace",
			};

			srcTranslator.Translate (file, options, sw);
			Console.WriteLine (sw.ToString ());
		}
	}
}
