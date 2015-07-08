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
			var tu = clang.Cursor_getTranslationUnit (cursor);
			var range = clang.getCursorExtent (cursor);
			var location = clang.getCursorLocation (cursor);

			CXFile file;
			uint line, column, offset;

			clang.getFileLocation (location, out file, out line, out column, out offset);
			string filePath = clang.getFileName (file).ToString ();

			uint offset1 = 0;
			uint offset2 = 0;
			using (var tg = TokenGroup.GetTokens (tu, range)) {

				for (int i = 0; i < tg.Tokens.Length; i++) {
					var t = tg.Tokens [i];
					offset1 = GetOffset (tu, t);

					if (clang.getTokenSpelling (tu, t).ToString () == "{")
						break;
				}

				for (int i = tg.Tokens.Length - 1; i >= 0; i--) {
					var t = tg.Tokens [i];
					offset2 = GetOffset (tu, t);

					if (clang.getTokenSpelling (tu, t).ToString () == "}")
						break;
				}
			}

			// We have to read bytes first and then convert them to utf8 string
			// because .net string is utf16 char array, but clang handles utf8 src text only.
			// clang's offset means byte offset (not utf16 char offset)

			uint count = offset2 - offset1 - 1; // without { and }
			offset1 ++; // move from open {
			byte[] text = new byte[count];
			using(FileStream fs = File.OpenRead(filePath)) {
				fs.Seek (offset1, SeekOrigin.Begin);
				fs.Read (text, 0, (int)count);
			}

			var str = Encoding.UTF8.GetString (text);
//			Console.WriteLine (str);

			return str;
		}

		static uint GetOffset (CXTranslationUnit tu, CXToken t)
		{
			uint c, l, offset;;
			CXFile f;

			CXSourceLocation location = clang.getTokenLocation (tu, t);
			clang.getFileLocation (location, out f, out l, out c, out offset);

			return offset;
		}
	}
}

