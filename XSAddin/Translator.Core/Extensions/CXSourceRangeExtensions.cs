using System;
using ClangSharp;

namespace Translator.Core
{
	public static class CXSourceRangeExtensions
	{
		public static CXSourceLocation Begin (this CXSourceRange range)
		{
			return clang.getRangeStart (range);
		}

		public static CXSourceLocation End (this CXSourceRange range)
		{
			return clang.getRangeEnd(range);
		}
	}
}

