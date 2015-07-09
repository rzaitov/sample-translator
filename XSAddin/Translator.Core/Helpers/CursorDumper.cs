using System;
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
			switch (cursor.kind) {
			case CXCursorKind.CXCursor_BinaryOperator:
				sb.AppendFormat ("{0} \"{1}\"", cursor.kind, GetBinaryOpCode(cursor));
				break;
			default:
				sb.AppendFormat ("{0} {1}", cursor.kind, cursor.ToString ());
				break;
			}

			if (!cursor.Location ().IsFromMainFile ())
				sb.Append (" from header");
			sb.AppendLine ();
		}

		static string GetBinaryOpCode (CXCursor cursor)
		{
			if (cursor.kind != CXCursorKind.CXCursor_BinaryOperator)
				throw new ArgumentException ();

			var children = cursor.GetChildren ();
			var first = children [0];
			var second = children [1];

			var sb = new StringBuilder ();
			Tuple<CXSourceLocation, CXSourceLocation> firstLocInfo = first.LocactionInfo ();
			Tuple<CXSourceLocation, CXSourceLocation> secondLocInfo = second.LocactionInfo ();
			PrintLocationInfo (firstLocInfo, sb);
			sb.AppendFormat (" {0}", TextHelper.GetText (first));

			sb.Append ("___");

			PrintLocationInfo (secondLocInfo, sb);
			sb.AppendFormat (" {0}", TextHelper.GetText (second));

			var firstRange = clang.getCursorExtent (first);
			var secondRange = clang.getCursorExtent (second);

			var opStart = clang.getRangeEnd (firstRange);
			var opEnd = clang.getRangeStart (secondRange);
			var opRange = clang.getRange (opStart, opEnd);

			Console.WriteLine ();
			Console.WriteLine (">>>");
			Console.WriteLine ("is from main file: {0}", cursor.Location().IsFromMainFile());
			Console.WriteLine ("full expression: {0}", TextHelper.GetText(cursor));
			Console.WriteLine ("left: {0} {1} {2}", TextHelper.GetText(first), first.kind, first.Location().IsFromMainFile());
			Console.WriteLine ("right: {0} {1} {2}", TextHelper.GetText(second), second.kind, second.Location().IsFromMainFile());
			Console.WriteLine (GetString(opStart, true));
			Console.WriteLine (GetString(opEnd, true));

			string opcode = TextHelper.GetTextBetween (opStart, opEnd).Trim ();
			Console.WriteLine (opcode);
			return opcode;


			CXTranslationUnit tu = clang.Cursor_getTranslationUnit (cursor);
			using (var tg = TokenGroup.GetTokens (tu, /*cursor.Extent()*/ opRange)) {
				Console.WriteLine (">>>");
				Console.WriteLine ("{0} {1}", tg.Tokens.Length, TextHelper.GetText(cursor));
				Console.WriteLine ("opRange {0}-{1}", GetString(opStart), GetString(opEnd));
				for (int i = 0; i < tg.Tokens.Length; i++) {
					CXToken t = tg.Tokens [i];
					var extent = t.Extent (tu);
					string startInfo = GetString (extent.Begin());
					string endInfo = GetString (extent.End ());
					Console.WriteLine ("{0} {1} {2}-{3}", t.Kind (), clang.getTokenSpelling (tu, t), startInfo, endInfo);
				}
				Console.WriteLine ();

//				var token = tg.Tokens [0];
//				if (clang.getTokenKind (token) != CXTokenKind.CXToken_Punctuation)
//					throw new InvalidProgramException ();
//
//				return clang.getTokenSpelling (tu, token).ToString ();
			}

			return sb.ToString ();
		}

		static void PrintLocationInfo (Tuple<CXSourceLocation, CXSourceLocation> locInfo, StringBuilder sb)
		{
			var begin = locInfo.Item1.FileLocation ();
			var end = locInfo.Item2.FileLocation ();
			sb.AppendFormat ("{0},{1},{2}–{3},{4},{5}", begin.Item2, begin.Item3, begin.Item4, end.Item2, end.Item3, end.Item4);
		}

		static string GetString (CXSourceLocation location, bool showFile = false)
		{
			Tuple<string, int, int, int> info = location.ExpansionLocation ();
			var sb = new StringBuilder ();
			sb.AppendFormat ("{0},{1},{2}", info.Item2, info.Item3, info.Item4);
			if (showFile)
				sb.Append (info.Item1);
			return sb.ToString ();
		}
	}
}