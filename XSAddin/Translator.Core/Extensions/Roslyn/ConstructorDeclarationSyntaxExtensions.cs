using System;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Translator.Core
{
	public static class ConstructorDeclarationSyntaxExtensions
	{
		public static ConstructorDeclarationSyntax WithPublicKeyword (this ConstructorDeclarationSyntax syntax)
		{
			return syntax.AddModifiers (SF.Token (SyntaxKind.PublicKeyword));
		}

		public static ConstructorDeclarationSyntax WithStaticKeyword (this ConstructorDeclarationSyntax syntax)
		{
			return syntax.AddModifiers (SF.Token (SyntaxKind.StaticKeyword));
		}
	}
}

