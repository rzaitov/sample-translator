using System;
using ClangSharp;

namespace Translator.Core
{
	public static class CXTokenExtensions
	{
		public static CXTokenKind Kind (this CXToken token)
		{
			return clang.getTokenKind (token);
		}

		public static string Spellings(this CXToken token, CXTranslationUnit tu)
		{
			return clang.getTokenSpelling (tu, token).ToString ();
		}

		public static CXSourceLocation Location (this CXToken token, CXTranslationUnit tu)
		{
			return clang.getTokenLocation (tu, token);
		}

		public static CXSourceRange Extent (this CXToken token, CXTranslationUnit tu)
		{
			return clang.getTokenExtent (tu, token);
		}
	}
}

