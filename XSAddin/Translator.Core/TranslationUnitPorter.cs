﻿using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;

using ClangSharp;

using Mono.Cecil;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.CSharp.Formatting;

using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Translator.Core
{
	public class TranslationUnitPorter
	{
		readonly IBindingLocator bindingLocator;
		readonly CXCursor translationUnit;
		CompilationUnitSyntax cu;

		ObjCImplementationDeclContext ImplContext { get; set; }
		ObjCCategoryImplDeclContext CategoryImplContext { get; set; }

		ObjCTypeNamePrettifier ObjCPrettifier { get; set; }

		public TranslationUnitPorter (CXCursor translationUnit, string ns, IBindingLocator bindingLocator)
		{
			if (translationUnit.kind != CXCursorKind.CXCursor_TranslationUnit)
				throw new ArgumentException ();

			if (string.IsNullOrWhiteSpace (ns))
				throw new ArgumentException ();

			if (bindingLocator == null)
				throw new ArgumentNullException ();

			this.translationUnit = translationUnit;
			this.bindingLocator = bindingLocator;

			cu = SyntaxFactory.CompilationUnit ();

			IEnumerable<UsingDirectiveSyntax> usings = CreateUsings ();
			foreach (var u in usings)
				cu = cu.AddUsings (u);

			var nsDecl = CreateNamespace (ns);
			foreach (var c in PortClasses ())
				nsDecl = nsDecl.AddMembers (c);

			cu = cu.AddMembers (nsDecl);
		}

		IEnumerable<UsingDirectiveSyntax> CreateUsings ()
		{
			// TODO: generate usings acording to included files
			yield return CreateUsing ("System");
			yield return CreateUsing ("UIKit");
			yield return CreateUsing ("Foundation");
		}

		UsingDirectiveSyntax CreateUsing (string name)
		{
			return SyntaxFactory.UsingDirective (SyntaxFactory.IdentifierName (name));
		}

		NamespaceDeclarationSyntax CreateNamespace (string ns)
		{
			return SyntaxFactory.NamespaceDeclaration (SyntaxFactory.IdentifierName (ns));
		}

		IEnumerable<ClassDeclarationSyntax> PortClasses ()
		{
			var classes = new List<ClassDeclarationSyntax> ();
			IEnumerable<CXCursor> children = translationUnit.GetChildren ().Where (c => !IsFromHeader(c));

			var unrecognized = new List<CXCursor> ();
			foreach (var c in children) {
				ImplContext = null;
				if (c.kind == CXCursorKind.CXCursor_ObjCImplementationDecl) {
					classes.Add (PortClass (c));
				} else if (c.kind == CXCursorKind.CXCursor_ObjCCategoryImplDecl) {
					classes.Add (PortCategory (c));
				} else {
					unrecognized.Add (c);
				}
			}

			// TODO: warn about unrecognized cursors

			return classes;
		}

		ClassDeclarationSyntax PortClass (CXCursor cursor)
		{
			if (cursor.kind != CXCursorKind.CXCursor_ObjCImplementationDecl)
				throw new ArgumentException ();

			ImplContext = new ObjCImplementationDeclContext (cursor);
			ObjCPrettifier = new ObjCTypeNamePrettifier (ImplContext.ClassName);

			ClassDeclarationSyntax classDecl = SF.ClassDeclaration (ImplContext.ClassName);
			classDecl = classDecl.AddModifiers (SF.Token (SyntaxKind.PublicKeyword));
			classDecl = classDecl.AddBaseListTypes (SF.SimpleBaseType (SF.ParseTypeName(ImplContext.SuperClassName)));

			var unrecognized = new List<CXCursor> ();
			IEnumerable<CXCursor> children = cursor.GetChildren ();
			foreach (var m in children) {
				if (m.kind == CXCursorKind.CXCursor_ObjCInstanceMethodDecl || m.kind == CXCursorKind.CXCursor_ObjCClassMethodDecl)
					classDecl = classDecl.AddMembers(PortMethod (m));
				else
					unrecognized.Add (m);
			}

			// TODO: warn about unrecognized cursors

			return classDecl;
		}

		ClassDeclarationSyntax PortCategory (CXCursor cursor)
		{
			if (cursor.kind != CXCursorKind.CXCursor_ObjCCategoryImplDecl)
				throw new ArgumentException ();

			CategoryImplContext = new ObjCCategoryImplDeclContext (cursor);
			ObjCPrettifier = new ObjCTypeNamePrettifier (CategoryImplContext.ExtendedClassName);

			string className = string.Format ("{0}{1}Extensions", CategoryImplContext.ExtendedClassName, CategoryImplContext.CategoryName);
			ClassDeclarationSyntax classDecl = SF.ClassDeclaration (className);
			classDecl = classDecl.AddModifiers (SF.Token (SyntaxKind.PublicKeyword), SF.Token (SyntaxKind.StaticKeyword));

			var unrecognized = new List<CXCursor> ();
			IEnumerable<CXCursor> children = cursor.GetChildren ();
			foreach (var m in children) {
				if (m.kind == CXCursorKind.CXCursor_ObjCInstanceMethodDecl || m.kind == CXCursorKind.CXCursor_ObjCClassMethodDecl)
					classDecl = classDecl.AddMembers(PortCategoryMethod (m));
				else
					unrecognized.Add (m);
			}

			// TODO: warn about unrecognized cursors

			return classDecl;
		}

		MethodDeclarationSyntax PortCategoryMethod (CXCursor cursor)
		{
			var objcMethod = new ObjCMethod (cursor);
			IEnumerable<Tuple<string, string>> mParams = FetchParamInfos (cursor);

			var thisParam = new Tuple<string, string> (CategoryImplContext.ExtendedClassName, "self");
			mParams = (new Tuple<string, string>[] { thisParam }).Concat (mParams);

			string retTypeName = ObjCPrettifier.Prettify (objcMethod.ReturnType.ToString ());
			string methodName = MethodHelper.ConvertToMehtodName (objcMethod.Selector);

			var mb = new MethodBuilder ();
			MethodDeclarationSyntax mDecl = mb.BuildExtensionMethod (retTypeName, methodName, mParams);

			IEnumerable<CXCursor> children = cursor.GetChildren ();
			var compoundStmt = children.First (c => c.kind == CXCursorKind.CXCursor_CompoundStmt);
			return AddBody (compoundStmt, mDecl);
		}

		BaseMethodDeclarationSyntax PortMethod (CXCursor cursor)
		{
			var objcMethod = new ObjCMethod (cursor);
			IEnumerable<Tuple<string, string>> mParams = FetchParamInfos (cursor);

			MethodDefinition mDef;
			MethodDeclarationSyntax mDecl = null;
			ConstructorDeclarationSyntax ctorDecl = null;

			var mb = new MethodBuilder ();
			var isBound = bindingLocator.TryFindMethod (ImplContext.SuperClassName, objcMethod.Selector, out mDef);
			if (isBound) {
				if (mDef.IsConstructor)
					ctorDecl = mb.BuildCtor (mDef, ImplContext.ClassName, mParams);
				else
					mDecl = mb.BuildDeclaration (mDef, mParams);
			} else {
				if (objcMethod.IsInitializer)
					ctorDecl = mb.BuildCtor (ImplContext.ClassName, mParams);
				else
					mDecl = BuildDefaultDeclaration (objcMethod, mParams);
			}

			IEnumerable<CXCursor> children = cursor.GetChildren ();
			var compoundStmt = children.First (c => c.kind == CXCursorKind.CXCursor_CompoundStmt);

			if (ctorDecl != null)
				return AddBody (compoundStmt, ctorDecl);
			else
				return AddBody (compoundStmt, mDecl);
		}

		MethodDeclarationSyntax BuildDefaultDeclaration (ObjCMethod objcMethod, IEnumerable<Tuple<string, string>> mParams)
		{
			string retTypeName = ObjCPrettifier.Prettify (objcMethod.ReturnType.ToString ());
			string methodName = MethodHelper.ConvertToMehtodName (objcMethod.Selector);

			var mb = new MethodBuilder ();
			MethodDeclarationSyntax mDecl = mb.BuildDeclaration (retTypeName, methodName, mParams);

			if (objcMethod.IsStatic)
				mDecl = mDecl.WithStaticKeyword ();

			return mDecl;
		}

		IEnumerable<Tuple<string, string>> FetchParamInfos (CXCursor methodCursor)
		{
			return methodCursor.GetChildren ()
				.Where (c => c.kind == CXCursorKind.CXCursor_ParmDecl)
				.Select (CreateParamInfo);
		}

		Tuple<string, string> CreateParamInfo (CXCursor parmDecl)
		{
			string paramName = parmDecl.ToString ();
//			Console.WriteLine ("Debug !!!!");
//			Console.WriteLine (parmDecl.ToString ());
			CXType type = clang.getCursorType (parmDecl);
//			Console.WriteLine ("type:      {0}", type.ToString ());
//			Console.WriteLine ("type kind: {0}", type.kind);
			var pointee = clang.getPointeeType (type);
//			Console.WriteLine ("pointee type:   {0}", pointee);
//			Console.WriteLine ("pointee kind:   {0}", pointee.kind);
			string typeName = pointee.kind != CXTypeKind.CXType_Invalid 
				? ObjCPrettifier.Prettify (pointee.ToString ())
				: ObjCPrettifier.Prettify (type.ToString ());

			return new Tuple<string, string> (typeName, paramName);
		}

		ParameterSyntax PortParameter (CXCursor parmDecl)
		{
			string paramName = parmDecl.ToString ();
			CXCursor typeRef = parmDecl.GetChildren ().First (); // CXCursor_TypeRef
			string typeName = typeRef.ToString ();

			return SyntaxFactory.Parameter(SyntaxFactory.Identifier(paramName))
				.WithType(SyntaxFactory.ParseTypeName(ObjCPrettifier.Prettify(typeName)));
		}

		MethodDeclarationSyntax AddBody (CXCursor compountStmt, MethodDeclarationSyntax mDecl)
		{
			mDecl = mDecl.AddBodyStatements (new StatementSyntax[0]);
			SyntaxToken cl = mDecl.Body.CloseBraceToken.WithLeadingTrivia (FetchTrivias (compountStmt));
			return mDecl.ReplaceToken(mDecl.Body.CloseBraceToken, cl);
		}

		ConstructorDeclarationSyntax AddBody (CXCursor compountStmt, ConstructorDeclarationSyntax ctorDecl)
		{
			ctorDecl = ctorDecl.AddBodyStatements (new StatementSyntax[0]);
			SyntaxToken cl = ctorDecl.Body.CloseBraceToken.WithLeadingTrivia (FetchTrivias (compountStmt));
			return ctorDecl.ReplaceToken(ctorDecl.Body.CloseBraceToken, cl);
		}

		IEnumerable<SyntaxTrivia> FetchTrivias (CXCursor compountStmt)
		{
			string methodBody = MethodHelper.GetTextFromCompoundStmt (compountStmt);
			IEnumerable<string> lines = MethodHelper.Comment (methodBody);
			return lines.Select (l => SyntaxFactory.SyntaxTrivia (SyntaxKind.SingleLineCommentTrivia, l));
		}

		static bool IsFromHeader (CXCursor cursor)
		{
			return !(cursor.IsFromMainFile () || cursor.kind == CXCursorKind.CXCursor_TranslationUnit);
		}

		public string Generate()
		{
			var ws = new AdhocWorkspace ();
//			OptionSet options = ws.Options;
//			options = options.WithChangedOption (CSharpFormattingOptions.);
			var formattedNode = Formatter.Format (cu, ws);

			StringBuilder sb = new StringBuilder();
			using (StringWriter writer = new StringWriter (sb))
				formattedNode.WriteTo (writer);

			string result = sb.ToString ();
			return result;
		}
	}
}