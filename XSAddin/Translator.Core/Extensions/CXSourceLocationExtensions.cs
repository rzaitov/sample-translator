using System;
using ClangSharp;

namespace Translator.Core
{
	public static class CXSourceLocationExtensions
	{
		public static Tuple<string, int, int, int>  SpellingLocation (this CXSourceLocation location)
		{
			CXFile file;
			uint line, col, offset;

			clang.getSpellingLocation(location, out file, out line, out col, out offset);

			string filePath = clang.getFileName (file).ToString ();
			return new Tuple<string, int, int, int> (filePath, (int)line, (int)col, (int)offset);
		}

		public static Tuple<string, int, int, int>  FileLocation (this CXSourceLocation location)
		{
			CXFile file;
			uint line, col, offset;

			clang.getFileLocation(location, out file, out line, out col, out offset);

			string filePath = clang.getFileName (file).ToString ();
			return new Tuple<string, int, int, int> (filePath, (int)line, (int)col, (int)offset);
		}

		public static Tuple<string, int, int, int>  ExpansionLocation (this CXSourceLocation location)
		{
			CXFile file;
			uint line, col, offset;

			clang.getExpansionLocation(location, out file, out line, out col, out offset);

			string filePath = clang.getFileName (file).ToString ();
			return new Tuple<string, int, int, int> (filePath, (int)line, (int)col, (int)offset);
		}

		public static bool IsFromMainFile (this CXSourceLocation location)
		{
			return clang.Location_isFromMainFile (location) > 0;
		}
	}
}

