using System;

using NUnit.Framework;
using ClangSharp;
using Translator.Core;
using System.Linq;

namespace Translator.Test
{
	[TestFixture]
	public class MethodExplorer
	{
		[Test]
		public void FindMethodDecl ()
		{
			const string headerContent = @"
#import <Foundation/Foundation.h>
@interface Person : NSObject
- (void)Do:(NSString *)str;
@end";

			const string codeContent = @"
#import ""Person.h""
@implementation Person
- (void)Do:(NSString *)str
{
}
@end";
			
			var manager = new ClangManager ();
			manager.StoreFile (headerContent, "Person.h");
			string codePath = manager.StoreFile (codeContent, "Person.m");
			CXCursor root = manager.GetRootCursor (codePath);

			var items = root.GetChildren ();
			var personDecl = items.Single (c => c.kind == CXCursorKind.CXCursor_ObjCInterfaceDecl);
			var personImpl = items.Single (c => c.kind == CXCursorKind.CXCursor_ObjCImplementationDecl);

			var methodDecl = personDecl.GetChildren ().Single (c => c.kind == CXCursorKind.CXCursor_ObjCInstanceMethodDecl);
			var methodImpl = personImpl.GetChildren ().Single (c => c.kind == CXCursorKind.CXCursor_ObjCInstanceMethodDecl);

			Assert.False (clang.equalCursors (methodDecl, methodImpl) > 0);

			var canonical = clang.getCanonicalCursor (methodImpl);
			Assert.True (clang.equalCursors (canonical, methodDecl) > 0);
		}
	}
}

