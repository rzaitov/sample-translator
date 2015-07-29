using System;
using ClangSharp;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Translator.Core
{
	public class TokenGroup : IDisposable
	{
		public CXTranslationUnit TranslationUnit { get; private set; }
		IntPtr memory;
		uint count;

		public CXToken[] Tokens;

		private TokenGroup (CXTranslationUnit tu, IntPtr memory, uint count)
		{
			this.memory = memory;
			this.count = count;
			TranslationUnit = tu;

			Tokens = new CXToken[(int)count];
			for (int i = 0; i < count; i++)
				Tokens [i] = Marshal.PtrToStructure<CXToken> (memory + i * Marshal.SizeOf<CXToken> ());
		}

		public static TokenGroup GetTokens (CXTranslationUnit tu, CXSourceRange range)
		{
			IntPtr tokens;
			uint count;
			clang.tokenize (tu, range, out tokens, out count);

			return new TokenGroup (tu, tokens, count);
		}

		public void Dispose ()
		{
			disposeTokens(TranslationUnit, memory, count);
		}

		[DllImport ("libclang.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_disposeTokens")]
		public static extern void disposeTokens (CXTranslationUnit TU, IntPtr tokensArray, uint NumTokens);
	}
}