using System;
using ClangSharp;
using System.Linq;

namespace Translator.Core
{
	public class ObjCCategoryImplDeclContext
	{
		public CXCursor ObjCCategoryImpl { get; private set; }
		public CXCursor ExtendedClassRef { get; private set; }

		public string CategoryName {
			get {
				return ObjCCategoryImpl.ToString ();
			}
		}

		public string ExtendedClassName {
			get {
				return ExtendedClassRef.ToString ();
			}
		}

		public ObjCCategoryImplDeclContext (CXCursor categoryImplDecl)
		{
			if (categoryImplDecl.kind != CXCursorKind.CXCursor_ObjCCategoryImplDecl)
				throw new ArgumentException ();

			ObjCCategoryImpl = categoryImplDecl;
			ExtendedClassRef = GetExtendedClass (ObjCCategoryImpl);
		}

		static CXCursor GetExtendedClass (CXCursor cursor)
		{
			return cursor.GetChildren ().Where (c => c.kind == CXCursorKind.CXCursor_ObjCClassRef).Single ();
		}
	}
}

