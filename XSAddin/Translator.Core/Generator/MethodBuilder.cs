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

		public MethodDeclarationSyntax BuildDeclaration (MethodDefinition baseMethodDef, IEnumerable<Tuple<string, string>> paramsInfo)
		{
			if (baseMethodDef == null || baseMethodDef.IsConstructor)
				throw new ArgumentNullException ();

			MethodDeclarationSyntax mDecl = CreateDeclaration (baseMethodDef.ReturnType.Name, baseMethodDef.Name);
			mDecl = mDecl.AddModifiers (FetchModifiers (baseMethodDef));

			ParameterSyntax[] parameters = BuildParameters (ReplaceTypes (baseMethodDef, paramsInfo));
			mDecl = mDecl.AddParameterListParameters (parameters);

			return mDecl;
		}

		public ConstructorDeclarationSyntax BuildCtor (MethodDefinition baseCtorDef, string className, IEnumerable<Tuple<string, string>> paramsInfo)
		{
			if (baseCtorDef == null || !baseCtorDef.IsConstructor)
				throw new ArgumentNullException ();

			ConstructorDeclarationSyntax ctorDecl = SF.ConstructorDeclaration (className);
			ctorDecl = ctorDecl.AddModifiers (FetchModifiers (baseCtorDef));

			ParameterSyntax[] parameters = BuildParameters (ReplaceTypes (baseCtorDef, paramsInfo));
			ctorDecl = ctorDecl.AddParameterListParameters (parameters);

			return ctorDecl;
		}

		public ConstructorDeclarationSyntax BuildCtor (string className, IEnumerable<Tuple<string, string>> paramsInfo)
		{
			ConstructorDeclarationSyntax ctorDecl = SF.ConstructorDeclaration (className)
				.AddModifiers(SF.Token (SyntaxKind.PublicKeyword));

			ParameterSyntax[] parameters = BuildParameters (paramsInfo);
			ctorDecl = ctorDecl.AddParameterListParameters (parameters);

			return ctorDecl;
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

			if (boundedMethodDef.IsStatic)
				modifiers.Add (SF.Token (SyntaxKind.StaticKeyword));

			return modifiers.ToArray ();
		}

		IEnumerable<Tuple<string, string>> ReplaceTypes (MethodDefinition mDef, IEnumerable<Tuple<string, string>> paramsInfo)
		{
			IEnumerator<ParameterDefinition> typesEnumerator = ((IEnumerable<ParameterDefinition>)mDef.Parameters).GetEnumerator ();
			var paramsEnumerator = paramsInfo.GetEnumerator ();

			while (typesEnumerator.MoveNext ()) {
				if (!paramsEnumerator.MoveNext ())
					throw new ArgumentException ();

				string typeName = typesEnumerator.Current.ParameterType.Name;
				yield return new Tuple<string, string> (typeName, paramsEnumerator.Current.Item2);
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

