﻿using System;

using Translator.Core;

using NUnit.Framework;
using ClangSharp;
using System.Linq;

namespace Translator.Test
{
	[TestFixture]
	public class ObjCPropertyTest
	{
		class Context
		{
			public string Path { get; set; }
			public ClangManager Manager { get; set; }
		}

		[Test]
		public void SimpleReadWrite ()
		{
			const string headerContent = @"
#import <Foundation/Foundation.h>
@interface Person : NSObject
@property (nonatomic) NSString *name;
@end";

			const string codeContent = @"
#import ""Person.h""
@implementation Person
@end";

			ObjCPropertyDecl p = FetchProperty (headerContent, codeContent);

			Assert.AreEqual ("name", p.GetterName);
			Assert.AreEqual ("setName", p.SetterName);
			Assert.AreEqual (true, p.IsReadWrite);
			Assert.AreEqual (false, p.IsReadonly);
		}

		[Test]
		public void SimpleReadonly ()
		{
			const string headerContent = @"
#import <Foundation/Foundation.h>
@interface Person : NSObject
@property (nonatomic, readonly) NSString *name;
@end";

			const string codeContent = @"
#import ""Person.h""
@implementation Person
@end";

			ObjCPropertyDecl p = FetchProperty (headerContent, codeContent);

			Assert.AreEqual ("name", p.GetterName);
			Assert.AreEqual (null, p.SetterName);
			Assert.AreEqual (false, p.IsReadWrite);
			Assert.AreEqual (true, p.IsReadonly);
		}

		[Test]
		public void CustomAccessors ()
		{
			const string headerContent = @"
#import <Foundation/Foundation.h>
@interface Person : NSObject
@property (nonatomic, getter=personName, setter=setupPersonName) NSString *name;
@end";

			const string codeContent = @"
#import ""Person.h""
@implementation Person
@end";

			ObjCPropertyDecl p = FetchProperty (headerContent, codeContent);

			Assert.AreEqual ("personName", p.GetterName);
			Assert.AreEqual ("setupPersonName", p.SetterName);
			Assert.AreEqual (true, p.IsReadWrite);
			Assert.AreEqual (false, p.IsReadonly);
		}

		ObjCPropertyDecl FetchProperty(string header, string code)
		{
			var manager = new ClangManager ();
			manager.StoreFile (header, "Person.h");
			string codePath = manager.StoreFile (code, "Person.m");

			var root = manager.GetRootCursor (codePath);
			var prop = root.GetChildren ()
				.Single(c => c.kind == CXCursorKind.CXCursor_ObjCInterfaceDecl)
				.GetChildren()
				.Single (c => c.kind == CXCursorKind.CXCursor_ObjCPropertyDecl);

			return new ObjCPropertyDecl (prop);
		}
	}
}