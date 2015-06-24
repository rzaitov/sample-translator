using System;
using ClangSharp;

namespace Translator.Core
{
	public class ObjCTypeNamePrettifier
	{
		readonly string idTypeName;

		public ObjCTypeNamePrettifier (string idTypeName)
		{
			this.idTypeName = idTypeName;
		}

		public string Prettify (CXType type)
		{
			if(type.kind == CXTypeKind.CXType_ObjCId)
				return idTypeName;

			if (type.kind == CXTypeKind.CXType_Void)
				return "void";

			string typeName = type.ToString ();
			if (type.kind == CXTypeKind.CXType_Typedef && typeName == "CGFloat")
				return "nfloat";

			return type.ToString ();
		}
	}
}