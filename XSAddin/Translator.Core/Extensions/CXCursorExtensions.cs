using System;
using System.Collections.Generic;
using ClangSharp;
using System.Linq;
using System.IO;

namespace Translator.Core
{
	public static class CXCursorExtensions
	{
		class VisitorState
		{
			readonly List<CXCursor> children = new List<CXCursor> ();
			public IEnumerable<CXCursor> Children {
				get {
					return children;
				}
			}

			public CXChildVisitResult VisitorCallback (CXCursor cursor, CXCursor parent, IntPtr client_data)
			{
				children.Add (cursor);

				return CXChildVisitResult.CXChildVisit_Continue;
			}
		}

		public static IEnumerable<CXCursor> GetChildren (this CXCursor cursor)
		{
			var state = new VisitorState ();
			clang.visitChildren(cursor, state.VisitorCallback, new CXClientData());
			return state.Children;
		}

		public static CXCursor GetSuperClass (this CXCursor cursor)
		{
			CXCursor canonical = clang.getCanonicalCursor (cursor);
			return canonical.GetChildren ().Where (c => c.kind == CXCursorKind.CXCursor_ObjCSuperClassRef).First ();
		}

		public static CXSourceLocation Location (this CXCursor cursor)
		{
			return clang.getCursorLocation (cursor);
		}

		public static bool IsFromMainFile (this CXCursor cursor)
		{
			return clang.Location_isFromMainFile (clang.getCursorLocation (cursor)) > 0;
		}
	}
}

