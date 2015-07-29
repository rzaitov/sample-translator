using System;
using NUnit.Framework;
using Translator.Core;
using ClangSharp;

namespace Translator.Test
{
	[TestFixture]
	public class ObjCPropertyImplInfoTest
	{
		class Context {
			public ClangManager Manger { get; set; }

			public CXCursor Root { get; set; }
			public CXCursor InterfaceDecl { get; set; }
			public CXCursor InterfaceImpl { get; set; }

			public ObjCPropertyDecl PropDecl { get; set; }
			public ObjCPropertyImplInfo PropImplInfo { get; set; }
		}

		[Test]
		public void ReadWriteAutoProperty ()
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

			var context = Parse (headerContent, codeContent);

			Assert.That (!context.PropImplInfo.Getter.HasValue);
			Assert.That (!context.PropImplInfo.Setter.HasValue);

			Assert.That (context.PropImplInfo.ObjCSynthesizeDecl.HasValue);
			Assert.That (context.PropImplInfo.ObjCSynthesizeDecl.Value.ToString (), Is.EqualTo ("name"));
		}

		[Test]
		public void ReadWriteExplicitGetterProperty ()
		{
			const string headerContent = @"
#import <Foundation/Foundation.h>
@interface Person : NSObject
@property (nonatomic) NSString *name;
@end";

			const string codeContent = @"
#import ""Person.h""
@implementation Person
- (NSString *) name
{
	return @""Alan"";
}
@end";

			var context = Parse (headerContent, codeContent);

			Assert.That (context.PropImplInfo.Getter.HasValue);
			Assert.That (context.PropImplInfo.Getter.Value.ToString (), Is.EqualTo("name"));

			Assert.That (!context.PropImplInfo.Setter.HasValue);

			Assert.That (context.PropImplInfo.ObjCSynthesizeDecl.HasValue);
			Assert.That (context.PropImplInfo.ObjCSynthesizeDecl.Value.ToString() , Is.EqualTo("name"));
		}

		[Test]
		public void ReadWriteExplicitSetterProperty ()
		{
			const string headerContent = @"
#import <Foundation/Foundation.h>
@interface Person : NSObject
@property (nonatomic) NSString *name;
@end";

			const string codeContent = @"
#import ""Person.h""
@implementation Person
- (void) setName: (NSString *)newName
{
	_name = newName;
}
@end";
			
			var context = Parse (headerContent, codeContent);
			context.Root.Dump ();

			Assert.That (!context.PropImplInfo.Getter.HasValue);

			Assert.That (context.PropImplInfo.Setter.HasValue);
			Assert.That (context.PropImplInfo.Setter.Value.ToString(), Is.EqualTo("setName:"));

			Assert.That (context.PropImplInfo.ObjCSynthesizeDecl.HasValue);
			Assert.That (context.PropImplInfo.ObjCSynthesizeDecl.Value.ToString() , Is.EqualTo("name"));
		}

		[Test]
		public void ReadWriteExplicitAccessorsProperty ()
		{
			const string headerContent = @"
#import <Foundation/Foundation.h>
@interface Person : NSObject
@property (nonatomic, getter=firstName, setter=saveNewName) NSString *name;
@end";

			const string codeContent = @"
#import ""Person.h""
@implementation Person
- (NSString *) firstName
{
	return @""Alan"";
}
- (void) saveNewName: (NSString *)newName
{
	_name = newName;
}
@end";

			var context = Parse (headerContent, codeContent);
			context.Root.Dump ();

			Assert.That (context.PropImplInfo.Getter.HasValue);
			Assert.That (context.PropImplInfo.Getter.Value.ToString(), Is.EqualTo("firstName"));

			Assert.That (context.PropImplInfo.Setter.HasValue);
			Assert.That (context.PropImplInfo.Setter.Value.ToString(), Is.EqualTo("saveNewName:"));

			Assert.False (context.PropImplInfo.ObjCSynthesizeDecl.HasValue);
		}


		Context Parse (string header, string code)
		{
			var manager = new ClangManager ();
			manager.StoreFile (header, "Person.h");
			string codePath = manager.StoreFile (code, "Person.m");

			var root = manager.GetRootCursor (codePath);
			var interfaceDecl = root.Child (CXCursorKind.CXCursor_ObjCInterfaceDecl);
			var interfaceImpl = root.Child (CXCursorKind.CXCursor_ObjCImplementationDecl);

			var prop = interfaceDecl.Child (CXCursorKind.CXCursor_ObjCPropertyDecl);
			var propDecl = new ObjCPropertyDecl (prop);
			var propImplInfo = ObjCPropertyImplInfo.FindImplementation (propDecl, interfaceImpl);

			return new Context {
				Manger = manager,
				Root = root,
				InterfaceDecl = interfaceDecl,
				InterfaceImpl = interfaceImpl,

				PropDecl = propDecl,
				PropImplInfo = propImplInfo
			};
		}
	}
}

