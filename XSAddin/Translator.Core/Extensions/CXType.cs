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

		public static bool IsPtrToPtr (this CXType type)
		{
			CXType pointee = type.GetPointee ();
			if (pointee.kind == CXTypeKind.CXType_Invalid)
				return false;

			pointee = pointee.GetPointee ();
			if (pointee.kind == CXTypeKind.CXType_Invalid)
				return false;

			return true;
		}

		public static CXType GetPointee (this CXType type)
		{
			return clang.getPointeeType (type);
		}
	}
}