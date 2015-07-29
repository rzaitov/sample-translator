using System;
using ClangSharp;
using Translator.Core;
using System.Linq;

namespace Translator.Test
{
	public static class CXCursorExtensionsTest
	{
		public static CXCursor Child (this CXCursor cursor, CXCursorKind kind)
		{
			return cursor.GetChildren ().Single (c => c.kind == kind);
		}
	}
}