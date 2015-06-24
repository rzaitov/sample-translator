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

		public MethodDeclarationSyntax BuildDeclaration (MethodDefinition baseMethodDef, IEnumerable<ParameterSyntax> paramsInfo)
		{
			if (baseMethodDef == null || baseMethodDef.IsConstructor)
				throw new ArgumentNullException ();

			var retType = Convert (baseMethodDef.ReturnType);
			MethodDeclarationSyntax mDecl = SF.MethodDeclaration (retType, baseMethodDef.Name);
			mDecl = mDecl.AddModifiers (FetchModifiers (baseMethodDef));

			ParameterSyntax[] parameters = ReplaceTypes (baseMethodDef, paramsInfo).ToArray ();
			mDecl = mDecl.AddParameterListParameters (parameters);

			return mDecl;
		}

		public MethodDeclarationSyntax BuildDeclaration (TypeSyntax returnType, string name, IEnumerable<ParameterSyntax> paramsInfo)
		{
			MethodDeclarationSyntax mDecl = SF.MethodDeclaration (returnType, name);
			mDecl = mDecl.AddModifiers (SF.Token (SyntaxKind.PublicKeyword));
			mDecl = mDecl.AddParameterListParameters (paramsInfo.ToArray ());

			return mDecl;
		}

		public MethodDeclarationSyntax BuildExtensionMethod (TypeSyntax returnType, string name, IEnumerable<ParameterSyntax> paramsInfo)
		{
			MethodDeclarationSyntax mDecl = SF.MethodDeclaration (returnType, name);
			mDecl = mDecl.AddModifiers (SF.Token (SyntaxKind.PublicKeyword), SF.Token (SyntaxKind.StaticKeyword));

			ParameterSyntax[] parameterSyntax = paramsInfo.ToArray ();
			ParameterSyntax thisParam = parameterSyntax [0];
			parameterSyntax [0] = thisParam.WithModifiers (SF.TokenList ((SF.Token (SyntaxKind.ThisKeyword))));

			mDecl = mDecl.AddParameterListParameters (parameterSyntax);

			return mDecl;
		}

		public ConstructorDeclarationSyntax BuildCtor (MethodDefinition baseCtorDef, string className, IEnumerable<ParameterSyntax> paramsInfo)
		{
			if (baseCtorDef == null || !baseCtorDef.IsConstructor)
				throw new ArgumentNullException ();

			ConstructorDeclarationSyntax ctorDecl = SF.ConstructorDeclaration (className);
			ctorDecl = ctorDecl.AddModifiers (FetchModifiers (baseCtorDef));

			ParameterSyntax[] parameters = ReplaceTypes (baseCtorDef, paramsInfo).ToArray ();
			ctorDecl = ctorDecl.AddParameterListParameters (parameters);

			return ctorDecl;
		}

		public ConstructorDeclarationSyntax BuildCtor (string className, IEnumerable<ParameterSyntax> paramsInfo)
		{
			ConstructorDeclarationSyntax ctorDecl = SF.ConstructorDeclaration (className)
				.AddModifiers(SF.Token (SyntaxKind.PublicKeyword));

			ParameterSyntax[] parameters = paramsInfo.ToArray ();
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

		IEnumerable<ParameterSyntax> ReplaceTypes (MethodDefinition mDef, IEnumerable<ParameterSyntax> paramsInfo)
		{
			IEnumerator<ParameterDefinition> typesEnumerator = ((IEnumerable<ParameterDefinition>)mDef.Parameters).GetEnumerator ();
			var paramsEnumerator = paramsInfo.GetEnumerator ();

			while (typesEnumerator.MoveNext ()) {
				if (!paramsEnumerator.MoveNext ())
					throw new ArgumentException ();

				string typeName = typesEnumerator.Current.ParameterType.Name;
				yield return paramsEnumerator.Current.WithType (SF.ParseTypeName (typeName));
			}
		}

		TypeSyntax Convert(TypeReference typeRef)
		{
			switch (typeRef.FullName) {
				case "System.Void":
					return CommonTypes.VoidTypeSyntax;

				default:
					return SF.ParseTypeName (typeRef.Name);
			}

		}
	}
}