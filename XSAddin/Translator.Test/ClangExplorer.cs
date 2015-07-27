using System;

using Translator.Core;
using NUnit.Framework;
using ClangSharp;
using System.Linq;

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
			manager.StoreFile (headerContent, "CustomObject.h");
			string codePath = manager.StoreFile (codeContent, "CustomObject.m");
			CXTranslationUnit translationUnit = manager.BuildTranslationUnit (codePath);

			CXCursor cursor = clang.getTranslationUnitCursor (translationUnit);
			var arr = cursor.GetChildren ().Where (c => c.kind == CXCursorKind.CXCursor_ObjCInterfaceDecl && c.ToString () == "CustomObject").ToArray ();
			Assert.AreEqual (1, arr.Length);
		}
	}
}