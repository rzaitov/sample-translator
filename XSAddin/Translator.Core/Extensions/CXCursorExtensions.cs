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

		public static string GetBodyText (this CXCursor cursor)
		{
			CXSourceRange range = clang.getCursorExtent (cursor);
			CXSourceLocation start = clang.getRangeStart (range);
			CXSourceLocation end = clang.getRangeEnd (range);

			CXFile file;
			uint line1, line2;
			uint column1, column2;
			uint offset1, offset2;

			clang.getFileLocation (start, out file, out line1, out column1, out offset1);
			clang.getFileLocation (end, out file, out line2, out column2, out offset2);

			string filePath = clang.getFileName (file).ToString ();
			string result = File.ReadAllText (filePath).Substring ((int)offset1, (int)(offset2 - offset1 + 1));
			return result;
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

