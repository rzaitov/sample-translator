using System;
using ClangSharp;

namespace Translator.Core
{
	public static class CXTypeExtensions
	{
		public static void Dump (this CXType type)
		{
			Console.WriteLine ("type: {0}\tkind: {1}", type, type.kind);
		}
	}
}