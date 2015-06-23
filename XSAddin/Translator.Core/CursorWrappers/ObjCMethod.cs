using System;
using ClangSharp;

namespace Translator.Core
{
	public class ObjCMethod
	{
		readonly CXCursor cursor;

		string selector;
		public string Selector {
			get {
				selector = selector ?? cursor.ToString ();
				return selector;
			}
		}

		public CXType ReturnType {
			get {
				return clang.getCursorResultType (cursor);
			}
		}

		public bool IsStatic {
			get {
				return cursor.kind == CXCursorKind.CXCursor_ObjCClassMethodDecl;
			}
		}

		public bool IsInstance {
			get {
				return cursor.kind != CXCursorKind.CXCursor_ObjCInstanceMethodDecl;
			}
		}

		public bool IsInitializer {
			get {
				return Selector.StartsWith ("init");
			}
		}

		public ObjCMethod (CXCursor cursor)
		{
			AssertKind (cursor);
			this.cursor = cursor;
		}

		void AssertKind (CXCursor cursor)
		{
			if (cursor.kind != CXCursorKind.CXCursor_ObjCInstanceMethodDecl &&
			    cursor.kind != CXCursorKind.CXCursor_ObjCClassMethodDecl)
				throw new ArgumentException ();
		}
	}
}

