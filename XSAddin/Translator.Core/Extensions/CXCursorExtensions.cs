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

		public static bool IsFromMainFile (this CXCursor cursor)
		{
			return clang.Location_isFromMainFile (clang.getCursorLocation (cursor)) > 0;
		}

		public static void Dump (this CXCursor cursor)
		{
			var sb = new StringBuilder ();
			Dump (cursor, sb, 0, 0);
			Console.WriteLine (sb.ToString ());
		}

		static void Dump (CXCursor cursor, StringBuilder sb, int level, int mask)
		{
			for (int i = 1; i <= level; i++) {
				if (IsSet (mask, level - i)) {
					if (i == level)
						sb.Append ("|-");
					else
						sb.Append ("| ");
				} else {
					if (i == level)
						sb.Append ("`-");
					else
						sb.Append ("  ");
				}
			}
			sb.AppendFormat ("{0} {1}\n", cursor.kind, cursor.ToString ());

			CXCursor[] children = cursor.GetChildren ().ToArray();
			for (int i = 0; i < children.Length; i++)
				Dump (children[i], sb, level + 1, (mask << 1) | (i == children.Length - 1 ? 0 : 1));
		}

		static bool IsSet(int mask, int i)
		{
			int probe = 1 << i;
			return (mask & probe) == probe;
		}

		public static bool IsObjCClassMethod (this CXCursor cursor)
		{
			return cursor.kind == CXCursorKind.CXCursor_ObjCClassMethodDecl;
		}
	}
}

