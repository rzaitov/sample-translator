using System;

using ClangSharp;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp;

namespace Translator.Core
{
	public class ObjCTypePorter
	{
		readonly string idTypeName;

		public ObjCTypePorter (string idTypeName)
		{
			this.idTypeName = idTypeName;
		}

		public ParameterSyntax PortParameter (CXCursor paramDecl)
		{
			string paramName = paramDecl.ToString ();
			CXType type = clang.getCursorType (paramDecl);

			// We can't pass void as function parameter
			if (type.kind == CXTypeKind.CXType_Void)
				throw new ArgumentException ();

			var paramId = SF.Identifier (paramName);
			ParameterSyntax paramSyntax = SF.Parameter (paramId);

			if (type.kind == CXTypeKind.CXType_ObjCObjectPointer) {
				type = type.GetPointee ();
				TypeSyntax typeSyntax = PortType (type);
				return paramSyntax.WithType (typeSyntax);
			}

			if (type.kind == CXTypeKind.CXType_Pointer) {
				type = type.GetPointee ();

				if(type.kind == CXTypeKind.CXType_Void) // original type was void*
					return paramSyntax.WithType(CommonTypes.IntPtrTypeSyntax);

				TypeSyntax typeSyntax = PortType (type);
				return paramSyntax.WithType (typeSyntax).WithModifiers (SF.TokenList (SF.Token (SyntaxKind.OutKeyword)));
			}

			TypeSyntax ts = PortType (type);
			return paramSyntax.WithType (ts);
		}

		public TypeSyntax PortType (CXType type)
		{
			if (type.kind == CXTypeKind.CXType_Pointer)
				return CommonTypes.IntPtrTypeSyntax;

			if (type.kind == CXTypeKind.CXType_ObjCObjectPointer) {
				type = type.GetPointee ();
				return SF.ParseTypeName (type.ToString ());
			}

			if (type.kind == CXTypeKind.CXType_Void)
				return CommonTypes.VoidTypeSyntax;

			if (type.kind == CXTypeKind.CXType_Int)
				return CommonTypes.IntTypeSyntax;

			if (type.kind == CXTypeKind.CXType_Float)
				return CommonTypes.FloatTypeSyntax;

			if (type.kind == CXTypeKind.CXType_Double)
				return CommonTypes.DoubleTypeSyntax;

			return SF.ParseTypeName (Prettify (type));
		}

		string Prettify (CXType type)
		{
			if(type.kind == CXTypeKind.CXType_ObjCId)
				return idTypeName;

			string typeName = type.ToString ();
			if (type.kind == CXTypeKind.CXType_Typedef && typeName == "CGFloat")
				return "nfloat";

			return type.ToString ();
		}
	}
}