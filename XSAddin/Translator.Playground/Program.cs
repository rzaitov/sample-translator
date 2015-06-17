using System;

using ClangSharp;

using Translator.Core;

namespace Translator.Playground
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string[] clangArgs = new string[] {
				"-v",
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

			Console.WriteLine ("Hello World!");
			CXIndex index = clang.createIndex (1, 1);

			CXUnsavedFile unsavedFiles;
			CXTranslationUnit translationUnit = clang.parseTranslationUnit(index, "test/ViewController.m", clangArgs, clangArgs.Length, out unsavedFiles, 0, 0);
			if (translationUnit.Pointer == IntPtr.Zero)
				throw new InvalidOperationException ();

			CXString spell = clang.getTranslationUnitSpelling (translationUnit);

			CXCursor cursor = clang.getTranslationUnitCursor (translationUnit);

			TranslationUnitPorter porter = new TranslationUnitPorter (cursor, "TestNS");
			Console.WriteLine (porter.Generate ());
		}
	}
}
