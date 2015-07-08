using System;
using ClangSharp;
using System.Linq;

namespace Translator.Core
{
	public static class InheritanceChainHelper
	{
		public static bool TryGetSuperClassRef(this CXCursor interfaceDecl, out CXCursor superClassRef)
		{
			var super = interfaceDecl.GetChildren ().Where (c => c.kind == CXCursorKind.CXCursor_ObjCSuperClassRef);

			if (super.Any ()) {
				superClassRef = super.Single ();
				return true;
			}

			superClassRef = clang.getNullCursor ();
			return false;
		}

		public static bool TryGetSuperClassDecl (this CXCursor interfaceDecl, out CXCursor superClassInterfaceDecl)
		{
			CXCursor superRef;
			if (interfaceDecl.TryGetSuperClassRef (out superRef)) {
				superClassInterfaceDecl = clang.getTypeDeclaration (clang.getCursorType (superRef));
				return true;
			}

			superClassInterfaceDecl = clang.getNullCursor ();
			return false;
		}
	}
}

