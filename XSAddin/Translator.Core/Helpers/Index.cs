using System;
using System.Collections.Generic;
using ClangSharp;

namespace Translator.Core
{
	public class Index
	{
		readonly HashSet<string> index = new HashSet<string> ();

		public void Put (CXCursor cursor)
		{
			index.Add (clang.getCursorUSR (cursor).ToString ());
		}

		public bool Contains (CXCursor cursor)
		{
			return index.Contains (clang.getCursorUSR (cursor).ToString ());
		}
	}
}