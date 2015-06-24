using System;
using ClangSharp;
using System.Linq;

namespace Translator.Core
{
	public class ObjCImplementationDeclContext
	{
		public CXCursor ImplCursor { get; private set; }
		public CXCursor SuperClassRef { get; private set; }

		public string ClassName {
			get {
				return ImplCursor.ToString ();
			}
		}

		public string SuperClassName {
			get {
				return SuperClassRef.ToString ();
			}
		}

		public ObjCImplementationDeclContext (CXCursor cursor)
		{
			if (cursor.kind != CXCursorKind.CXCursor_ObjCImplementationDecl)
				throw new ArgumentException ();

			ImplCursor = cursor;
			SuperClassRef = GetSuperClass (cursor);
		}

		static CXCursor GetSuperClass (CXCursor cursor)
		{
			CXCursor canonical = clang.getCanonicalCursor (cursor);
			return canonical.GetChildren ().Where (c => c.kind == CXCursorKind.CXCursor_ObjCSuperClassRef).Single ();
		}
	}
}