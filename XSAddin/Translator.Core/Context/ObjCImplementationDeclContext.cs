using System;
using ClangSharp;
using System.Linq;

namespace Translator.Core
{
	public class ObjCImplementationDeclContext
	{
		public CXCursor DeclCursor { get; private set; }
		public CXCursor ImplCursor { get; private set; }

		public string ClassName {
			get {
				return ImplCursor.ToString ();
			}
		}

		public ObjCImplementationDeclContext (CXCursor cursor)
		{
			if (cursor.kind != CXCursorKind.CXCursor_ObjCImplementationDecl)
				throw new ArgumentException ();

			ImplCursor = cursor;
			DeclCursor = clang.getCanonicalCursor (cursor);
		}
	}
}