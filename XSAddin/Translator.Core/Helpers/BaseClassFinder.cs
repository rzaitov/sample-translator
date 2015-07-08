using System;
using ClangSharp;

namespace Translator.Core
{
	public class BaseClassFinder
	{
		readonly CXCursor interfaceDecl;
		public BaseClassFinder (CXCursor cursor)
		{
			if (cursor.kind == CXCursorKind.CXCursor_ObjCImplementationDecl)
				interfaceDecl = clang.getCanonicalCursor (cursor);
			else if (cursor.kind == CXCursorKind.CXCursor_ObjCInterfaceDecl)
				interfaceDecl = cursor;
			else
				throw new ArgumentException ();
		}

		public bool TryFindFirstBaseFromSystemHeaders(out CXCursor superClassInterfaceDecl)
		{
			return TryFindFirstBaseFromSystemHeaders (interfaceDecl, out superClassInterfaceDecl);
		}

		static bool TryFindFirstBaseFromSystemHeaders (CXCursor interfaceDecl, out CXCursor superClassInterfaceDecl)
		{
			if (!interfaceDecl.TryGetSuperClassDecl (out superClassInterfaceDecl))
				return false;

			CXSourceLocation location = superClassInterfaceDecl.Location ();
			bool isInSystemHeader = clang.Location_isInSystemHeader (location) > 0;

			if (isInSystemHeader)
				return true;

			interfaceDecl = superClassInterfaceDecl;
			return TryFindFirstBaseFromSystemHeaders (interfaceDecl, out superClassInterfaceDecl);
		}
	}
}