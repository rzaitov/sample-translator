using System;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Translator.Core
{
	public static class MethodDeclarationSyntaxExtensions
	{
		public static MethodDeclarationSyntax WithPublicKeyword (this MethodDeclarationSyntax syntax)
		{
			return syntax.AddModifiers (SF.Token (SyntaxKind.PublicKeyword));
		}

		public static MethodDeclarationSyntax WithStaticKeyword (this MethodDeclarationSyntax syntax)
		{
			return syntax.AddModifiers (SF.Token (SyntaxKind.StaticKeyword));
		}
	}
}

