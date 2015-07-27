using System;
using System.Collections.Generic;
using ClangSharp;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;

namespace Translator.Core
{
	public static class CXCursorExtensions
	{
		class VisitorState
		{
			public List<CXCursor> Children { get; private set; }

			public VisitorState ()
			{
				Children = new List<CXCursor> ();
			}

			public CXChildVisitResult VisitorCallback (CXCursor cursor, CXCursor parent, IntPtr client_data)
			{
				Children.Add (cursor);
				return CXChildVisitResult.CXChildVisit_Continue;
			}
		}

		public static List<CXCursor> GetChildren (this CXCursor cursor)
		{
			var state = new VisitorState ();
			clang.visitChildren(cursor, state.VisitorCallback, new CXClientData());
			return state.Children;
		}

		public static CXSourceLocation Location (this CXCursor cursor)
		{
			return clang.getCursorLocation (cursor);
		}

		public static CXSourceRange Extent (this CXCursor cursor)
		{
			return clang.getCursorExtent (cursor);
		}

		public static Tuple<CXSourceLocation, CXSourceLocation> LocationInfo (this CXCursor cursor)
		{
			var extent = cursor.Extent ();
			return new Tuple<CXSourceLocation, CXSourceLocation> (extent.Begin (), extent.End ());
		}

		public static bool IsFromMainFile (this CXCursor cursor)
		{
			return clang.Location_isFromMainFile (cursor.Location()) > 0;
		}

		public static void Dump (this CXCursor cursor)
		{
			var dumper = new CursorDumper ();
			Console.WriteLine (dumper.Dump (cursor));
		}

		public static bool IsObjCClassMethod (this CXCursor cursor)
		{
			return cursor.kind == CXCursorKind.CXCursor_ObjCClassMethodDecl;
		}
	}
}