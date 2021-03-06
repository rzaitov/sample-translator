﻿using System;
using System.Text;

using ClangSharp;
using System.IO;

namespace Translator.Core
{
	public class CursorDumper
	{
		readonly StringBuilder sb;

		public CursorDumper ()
		{
			sb = new StringBuilder ();
		}

		public string Dump (CXCursor cursor)
		{
			sb.Clear ();
			Dump (cursor, sb, 0, 0);
			return sb.ToString ();
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
			PrintCursorInfo (cursor, sb);

			CXCursor[] children = cursor.GetChildren ().ToArray();
			for (int i = 0; i < children.Length; i++)
				Dump (children[i], sb, level + 1, (mask << 1) | (i == children.Length - 1 ? 0 : 1));
		}

		static bool IsSet(int mask, int i)
		{
			int probe = 1 << i;
			return (mask & probe) == probe;
		}

		static void PrintCursorInfo (CXCursor cursor, StringBuilder sb)
		{
			sb.Append (BasicInfo (cursor));

			switch (cursor.kind) {
			case CXCursorKind.CXCursor_BinaryOperator:
				string opCode = GetBinaryOpCode (cursor);
				if (!string.IsNullOrWhiteSpace (opCode))
					sb.AppendFormat (" \"{0}\"", opCode);
				break;
			case CXCursorKind.CXCursor_ObjCPropertyDecl:
				sb.Append (' ').Append (GetPropertyAttributes (cursor));
				break;
			default:
				break;
			}

			sb.AppendFormat (" {0}", GetString (cursor.LocationInfo (), Show.LineColumn));
			sb.AppendLine ();
		}

		static string BasicInfo (CXCursor cursor)
		{
			var sb = new StringBuilder ();
			sb.Append (cursor.kind);

			string spelling = cursor.ToString ();
			if (!string.IsNullOrWhiteSpace (spelling))
				sb.AppendFormat (" {0}", spelling);

			return sb.ToString ();
		}

		static string GetBinaryOpCode (CXCursor cursor)
		{
			if (cursor.kind != CXCursorKind.CXCursor_BinaryOperator)
				throw new ArgumentException ();

			var children = cursor.GetChildren ();
			var left = children [0];
			var righ = children [1];

			Tuple<CXSourceLocation, CXSourceLocation> leftOpLoc = left.LocationInfo ();
			Tuple<CXSourceLocation, CXSourceLocation> rightOpLoc = righ.LocationInfo ();
			string opCode = TextHelper.GetTextBetween (leftOpLoc.Item2, rightOpLoc.Item1);
			return string.IsNullOrWhiteSpace (opCode) ? null : opCode.Trim ();
		}

		static string GetPropertyAttributes(CXCursor propDecl)
		{
			var attrs = (CXObjCPropertyAttrKind)clang.Cursor_getObjCPropertyAttributes (propDecl, 0);

			var sb = new StringBuilder ();
			sb.Append ('{');

			if (attrs.Equals (CXObjCPropertyAttrKind.CXObjCPropertyAttr_noattr))
				sb.Append ("noattr,");
			if (attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_readonly))
				sb.Append ("readonly,");
			if (attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_getter))
				sb.Append ("getter,");
			if (attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_assign))
				sb.Append ("assign,");
			if (attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_readwrite))
				sb.Append ("readwrite,");
			if (attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_retain))
				sb.Append ("retain,");
			if (attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_copy))
				sb.Append ("copy,");
			if (attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_nonatomic))
				sb.Append ("nonatomic,");
			if (attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_setter))
				sb.Append ("setter,");
			if (attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_atomic))
				sb.Append ("atomic,");
			if (attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_weak))
				sb.Append ("weak,");
			if (attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_strong))
				sb.Append ("strong,");
			if (attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_unsafe_unretained))
				sb.Append ("unsafe_unretained,");

			if (sb.Length > 1)
				sb.Length -= 1;

			sb.Append ('}');

			return sb.ToString ();
		}

		static string GetString (Tuple<CXSourceLocation, CXSourceLocation> locations, Show opt)
		{
			var sb = new StringBuilder ();
			var begin = locations.Item1.ExpansionLocation ();
			var end = locations.Item2.ExpansionLocation ();

			sb.Append (GetString (begin, opt & ~Show.FileName & ~Show.FilePath));
			sb.Append ('–');
			sb.Append (GetString (end, opt & ~Show.FileName & ~Show.FilePath));

			if (opt.HasFlag (Show.FilePath))
				sb.AppendFormat (" {0}", begin.Item1.Path);
			else if (opt.HasFlag (Show.FileName))
				sb.AppendFormat (" {0}", begin.Item1.FileName);

			return sb.ToString ();
		}

		static string GetString (Tuple<ClangFile, int, int, int> locationInof, Show opt)
		{
			var sb = new StringBuilder ();
			if (opt.HasFlag (Show.Line))
				sb.Append (locationInof.Item2).Append (',');
			if (opt.HasFlag (Show.Column))
				sb.Append (locationInof.Item3).Append (',');
			if (opt.HasFlag (Show.Offset))
				sb.Append (locationInof.Item4).Append (',');
			if (opt.HasFlag (Show.FilePath))
				sb.Append (locationInof.Item1.Path).Append (',');
			else if (opt.HasFlag (Show.FileName))
				sb.Append (Path.GetFileName (locationInof.Item1.FileName)).Append (',');

			if (sb.Length > 0)
				sb.Length -= 1;

			return sb.ToString ();
		}

		enum Show {
			Nothing = 0,
			Line = 1,
			Column = 1 << 1,
			LineColumn = Line | Column,
			Offset = 1 << 2,
			FileName = 1 << 3,
			FilePath = 1 << 4,
		}

	}
}