using System;
using ClangSharp;
using System.IO;
using System.Text;

namespace Translator.Core
{
	public static class TextHelper
	{
		public static string GetText(CXCursor cursor)
		{
			CXSourceRange range = clang.getCursorExtent(cursor);
			CXSourceLocation begin = clang.getRangeStart(range);
			CXSourceLocation end = clang.getRangeEnd(range);

			return GetTextBetween (begin, end);
		}

		public static string GetTextBetween (CXSourceLocation begin, CXSourceLocation end)
		{
			var beginInfo = begin.SpellingLocation ();
			var endInfo = end.SpellingLocation ();

			string filePath = beginInfo.Item1;
			int length = Math.Abs (endInfo.Item4 - beginInfo.Item4);
			int start = Math.Min (endInfo.Item4, beginInfo.Item4);

			byte[] text = new byte[length];
			using(FileStream fs = File.OpenRead(filePath)) {
				fs.Seek (start, SeekOrigin.Begin);
				fs.Read (text, 0, (int)length);
			}

			return Encoding.UTF8.GetString (text);
		}
	}
}

