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

			CXCursor cursor = manager.GetRootCursor (codePath);
			var arr = cursor.GetChildren ().Where (c => c.kind == CXCursorKind.CXCursor_ObjCInterfaceDecl && c.ToString () == "CustomObject").ToArray ();
			Assert.AreEqual (1, arr.Length);
		}

		[Test]
		public void SimpleProperty ()
		{
			string headerContent = @"
#import <Foundation/Foundation.h>
@interface Person : NSObject
@property (nonatomic) NSString *name;
@end";

			string codeContent = @"
#import ""Person.h""
@implementation Person
@end";

			var manager = new ClangManager ();
			manager.StoreFile (headerContent, "Person.h");
			string codePath = manager.StoreFile (codeContent, "Person.m");
			var root = manager.GetRootCursor (codePath);
			var interfaceDecl = root.GetChildren ().Single (c => c.kind == CXCursorKind.CXCursor_ObjCInterfaceDecl);

			var properties = interfaceDecl.GetChildren ().Where (c => c.kind == CXCursorKind.CXCursor_ObjCPropertyDecl).ToArray();
			Assert.That (properties.Length, Is.EqualTo (1));

			root.Dump ();
			var attrs = (CXObjCPropertyAttrKind)clang.Cursor_getObjCPropertyAttributes (properties[0], 0);
			Console.WriteLine (attrs);
		}
	}
}