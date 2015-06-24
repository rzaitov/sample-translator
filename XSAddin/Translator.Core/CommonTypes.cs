using System;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Translator.Core
{
	public static class CommonTypes
	{
		public static readonly TypeSyntax VoidTypeSyntax = SF.PredefinedType(SF.Token(SyntaxKind.VoidKeyword));
		public static readonly TypeSyntax BoolTypeSyntax = SF.PredefinedType(SF.Token(SyntaxKind.BoolKeyword));
		public static readonly TypeSyntax IntTypeSyntax = SF.PredefinedType(SF.Token(SyntaxKind.IntKeyword));
		public static readonly TypeSyntax FloatTypeSyntax = SF.PredefinedType(SF.Token(SyntaxKind.FloatKeyword));
		public static readonly TypeSyntax DoubleTypeSyntax = SF.PredefinedType(SF.Token(SyntaxKind.DoubleKeyword));

		public static readonly TypeSyntax IntPtrTypeSyntax = SF.ParseTypeName ("IntPtr");
	}
}

