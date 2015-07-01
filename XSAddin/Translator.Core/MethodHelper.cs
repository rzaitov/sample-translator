using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using ClangSharp;
using System.IO;

namespace Translator.Core
{
	public static class MethodHelper
	{
		public static string ConvertToMehtodName (string selector)
		{
			string[] items = selector.Split (new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
			string name = string.Join (string.Empty, items.Select (s => {
				var chars = s.ToCharArray ();
				chars [0] = char.ToUpper (chars [0]);
				return new string (chars);
			}));

			return name;
		}

		public static string GetTextFromCompoundStmt (CXCursor cursor)
		{
			var stmts = cursor.GetChildren ();

			CXSourceLocation start = clang.getCursorLocation (stmts.First ());
			CXSourceLocation end = clang.getRangeEnd (clang.getCursorExtent (stmts.Last ()));

			CXFile file;
			uint line1, line2;
			uint column1, column2;
			uint offset1, offset2;

			clang.getFileLocation (start, out file, out line1, out column1, out offset1);
			clang.getFileLocation (end, out file, out line2, out column2, out offset2);

			string filePath = clang.getFileName (file).ToString ();

			// We have to read bytes first and then convert them to utf8 string
			// because .net string is utf16 char array, but clang handles utf8 src text only.
			// clang's offset means byte offset (not utf16 char offset)

			uint count = offset2 - offset1 + 1;
			byte[] text = new byte[count];
			using(FileStream fs = File.OpenRead(filePath)) {
				fs.Seek (offset1, SeekOrigin.Begin);
				fs.Read (text, 0, (int)count);
			}

			return Encoding.UTF8.GetString (text);
		}

		public static IEnumerable<string> Comment (string code)
		{
			string[] lines = code.Split (new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var l in lines)
				yield return string.Format ("//{0}\n", l);
		}
	}
}

