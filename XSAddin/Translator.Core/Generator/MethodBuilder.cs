using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Cecil;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Translator.Core
{
	public class MethodBuilder
	{
		public MethodBuilder ()
		{
		}

		public MethodDeclarationSyntax BuildDeclaration (MethodDefinition boundedMethodDef, IEnumerable<string> paramNames)
		{
			if (boundedMethodDef == null)
				throw new ArgumentNullException ();

			MethodDeclarationSyntax mDecl = CreateDeclaration (boundedMethodDef.ReturnType.Name, boundedMethodDef.Name);
			mDecl = mDecl.AddModifiers (FetchModifiers (boundedMethodDef));

			ParameterSyntax[] parameters = BuildParameters (FetchParamsInfo (boundedMethodDef, paramNames));
			mDecl = mDecl.AddParameterListParameters (parameters);

			return mDecl;
		}

		SyntaxToken[] FetchModifiers (MethodDefinition boundedMethodDef)
		{
			var modifiers = new List<SyntaxToken> ();

			if (boundedMethodDef.IsPublic)
				modifiers.Add (SF.Token (SyntaxKind.PublicKeyword));
			else if (boundedMethodDef.IsFamily)
				modifiers.Add (SF.Token (SyntaxKind.ProtectedKeyword));

			if (boundedMethodDef.IsVirtual)
				modifiers.Add (SF.Token (SyntaxKind.OverrideKeyword));

			return modifiers.ToArray ();
		}

		IEnumerable<Tuple<string, string>> FetchParamsInfo (MethodDefinition mDef, IEnumerable<string> paramNames)
		{
			IEnumerator<ParameterDefinition> typesEnumerator = ((IEnumerable<ParameterDefinition>)mDef.Parameters).GetEnumerator ();
			var namesEnumerator = paramNames.GetEnumerator ();

			while (typesEnumerator.MoveNext ()) {
				if (!namesEnumerator.MoveNext ())
					throw new ArgumentException ();

				string typeName = typesEnumerator.Current.ParameterType.Name;
				yield return new Tuple<string, string> (typeName, namesEnumerator.Current);
			}
		}

		public MethodDeclarationSyntax BuildDefaultDeclaration (string returnTypeName, string name, IEnumerable<Tuple<string, string>> paramsInfo)
		{
			MethodDeclarationSyntax mDecl = CreateDeclaration (returnTypeName, name);
			mDecl = mDecl.AddModifiers (SF.Token (SyntaxKind.PublicKeyword));
			mDecl = mDecl.AddParameterListParameters (BuildParameters (paramsInfo));

			return mDecl;
		}

		MethodDeclarationSyntax CreateDeclaration (string returnTypeName, string name)
		{
			TypeSyntax returnType = SF.ParseTypeName (returnTypeName);
			return SF.MethodDeclaration (returnType, name);
		}

		ParameterSyntax[] BuildParameters (IEnumerable<Tuple<string, string>> paramsInfo)
		{
			ParameterSyntax[] paramList = paramsInfo.Select (BuildParameter).ToArray ();
			return paramList;
		}

		// (type, name)
		ParameterSyntax BuildParameter (Tuple<string, string> paramInfo)
		{
			return SF.Parameter (SF.Identifier (paramInfo.Item2)).WithType (SF.ParseTypeName (paramInfo.Item1));
		}
	}
}

