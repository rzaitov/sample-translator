using System;
using ClangSharp;
using System.Linq;

namespace Translator.Core
{
	public static class InheritanceChainHelper
	{
		public static bool TryGetSuperClassRef(CXCursor interfaceDecl, out CXCursor superClassRef)
		{
			if (interfaceDecl.kind != CXCursorKind.CXCursor_ObjCInterfaceDecl)
				throw new ArgumentException ();

			var super = interfaceDecl.GetChildren ().Where (c => c.kind == CXCursorKind.CXCursor_ObjCSuperClassRef);

			if (super.Any ()) {
				superClassRef = super.Single ();
				return true;
			}

			superClassRef = clang.getNullCursor ();
			return false;
		}

		public static bool TryGetSuperClassDecl (CXCursor interfaceDecl, out CXCursor superClassInterfaceDecl)
		{
			if (interfaceDecl.kind != CXCursorKind.CXCursor_ObjCInterfaceDecl)
				throw new ArgumentException ();

			CXCursor superRef;
			if (TryGetSuperClassRef (interfaceDecl, out superRef)) {
				superClassInterfaceDecl = clang.getTypeDeclaration (clang.getCursorType (superRef));
				return true;
			}

			superClassInterfaceDecl = clang.getNullCursor ();
			return false;
		}
	}
}

