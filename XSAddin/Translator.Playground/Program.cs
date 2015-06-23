using System;

using ClangSharp;

using Translator.Core;
using System.Linq;

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

			string file = "test/Cell.m";
//			string file = "/Users/rzaitov/Documents/Apps/Xamarin/apple-samples/AddressBookCocoa/AddressBookCocoa.m";
//			string file = "/Users/rzaitov/Documents/Apps/Xamarin/apple-samples/HomeKitCatalogCreatingHomesPairingandControllingAccessoriesandSettingUpTriggers/HMCatalog/Supporting Files/Utilities/UITableView+Updating.m";
//			string file = "/Users/rzaitov/Documents/Apps/Xamarin/apple-samples/LayoutManagerDemo/LayoutManagerDemo/main.m";

			Console.WriteLine ("Hello World!");
			CXIndex index = clang.createIndex (1, 1);

			CXUnsavedFile unsavedFiles;
			CXTranslationUnit translationUnit = clang.parseTranslationUnit(index, file, clangArgs, clangArgs.Length, out unsavedFiles, 0, 0);
			if (translationUnit.Pointer == IntPtr.Zero)
				throw new InvalidOperationException ();

			CXString spell = clang.getTranslationUnitSpelling (translationUnit);

			CXCursor cursor = clang.getTranslationUnitCursor (translationUnit);

			cursor.Dump ();
//			var f = cursor.GetChildren ().First(c => c.kind == CXCursorKind.CXCursor_ObjCImplementationDecl).GetChildren().First (c => c.ToString() == "initWithCoder:");
//			f.Dump ();
//
//			var def = clang.getCursorDefinition (f);
//			def.Dump ();
//
//			CXType type = clang.getCursorResultType (f);
//			Console.WriteLine ("type {0}", type.ToString ());
//			Console.WriteLine ("type kind {0}", type.kind);

//
//			var cs = f.GetChildren ().First (c => c.kind == CXCursorKind.CXCursor_CompoundStmt);
//			var ch = cs.GetChildren ().ToArray ();
//			Console.WriteLine (ch.Length);
//
//			cs.Dump ();

			var pathLocator = new XamarinPathLocator ();
			string xi = pathLocator.GetAssemblyPath (Platform.iOS);

			var locator = new BindingLocator (new string[] { xi });

			TranslationUnitPorter porter = new TranslationUnitPorter (cursor, "TestNS", locator);
			Console.WriteLine (porter.Generate ());
		}
	}
}
