using System;

using Translator.Core;
using NUnit.Framework;
using ClangSharp;

namespace Translator.Test
{
	/// <summary>
	/// This is not a test fixture this this a way to explore and document clang bindings
	/// </summary>
	[TestFixture]
	public class ClangExplorer
	{
		[Test]
		public void SimpleClassDeclaration ()
		{
			string headerContent = @"
#import <Foundation/Foundation.h>

@interface CustomObject : NSObject
@end";

			string codeContent = @"
#import ""CustomObject.h""

@implementation CustomObject
@end";

			var manager = new ClangManager ();
			var headerPath = manager.StoreFile (headerContent, "CustomObject.h");
			var codePath = manager.StoreFile (codeContent, "CustomObject.m");

			CXIndex index = clang.createIndex (1, 1);
			CXUnsavedFile unsavedFiles;
			uint options = (uint)CXTranslationUnit_Flags.CXTranslationUnit_DetailedPreprocessingRecord;
			CXTranslationUnit translationUnit = clang.parseTranslationUnit(index, codePath, clangArgs, clangArgs.Length, out unsavedFiles, 0, options);

		}
	}
}