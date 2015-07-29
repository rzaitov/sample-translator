using System;
using ClangSharp;
using System.Text;

namespace Translator.Core
{
	public class CursorTokenDumper
	{
		public string Dump (CXCursor cursor)
		{
			var sb = new StringBuilder ();

			var tu = clang.Cursor_getTranslationUnit (cursor);
			var range = clang.getCursorExtent (cursor);

			using (var tg = TokenGroup.GetTokens (tu, range)) {
				for (int i = 0; i < tg.Tokens.Length; i++) {
					CXToken t = tg.Tokens [i];
					var tokenRange = t.Extent(tu);

					CXSourceLocation begin = tokenRange.Begin ();
					CXSourceLocation end = tokenRange.End ();

					var beginSpellLoc = begin.SpellingLocation ();
					var endSpellLoc = end.SpellingLocation ();

					sb.Append(t.Kind())
						.Append(' ')
						.Append (t.Spellings (tu))
						.Append (' ')
						.Append (beginSpellLoc.Item2)
						.Append (',')
						.Append (beginSpellLoc.Item3)
						.Append ('–')
						.Append (endSpellLoc.Item2)
						.Append (',')
						.Append (endSpellLoc.Item3)
						.AppendLine ();
				}
			}

			return sb.ToString ();
		}
	}
}