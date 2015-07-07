﻿using System;
using ClangSharp;
using System.Linq;

namespace Translator.Core
{
	public class ObjCImplementationDeclContext
	{
		public CXCursor DeclCursor { get; private set; }
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
			DeclCursor = clang.getCanonicalCursor (cursor);
			SuperClassRef = GetSuperClass (DeclCursor);
		}

		static CXCursor GetSuperClass (CXCursor interfaceDecl)
		{
			return interfaceDecl.GetChildren ().Where (c => c.kind == CXCursorKind.CXCursor_ObjCSuperClassRef).Single ();
		}

		public CXCursor GetFirstParentFromSystemFramework ()
		{
			return FindFirstParentFromSystemFramework (DeclCursor);
		}

		static CXCursor FindFirstParentFromSystemFramework (CXCursor declCursor)
		{
			var super = GetSuperClass (declCursor);
			super = clang.getTypeDeclaration (clang.getCursorType (super));
			if (clang.Location_isInSystemHeader (super.Location ()) > 0)
				return super;

			return FindFirstParentFromSystemFramework (super);
		}
	}
}