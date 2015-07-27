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
			var beginInfo = begin.ExpansionLocation ();
			var endInfo = end.ExpansionLocation ();

			string filePath = beginInfo.Item1.Path;
			int start = beginInfo.Item4;
			int length = endInfo.Item4 - beginInfo.Item4;

			if (length < 0)
				return null;

			byte[] text = new byte[length];
			using(FileStream fs = File.OpenRead(filePath)) {
				fs.Seek (start, SeekOrigin.Begin);
				fs.Read (text, 0, (int)length);
			}

			return Encoding.UTF8.GetString (text);
		}
	}
}

