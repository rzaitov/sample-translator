using System;
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
		readonly TranslatorOptions options;

		ObjCImplementationDeclContext ImplContext { get; set; }
		ObjCCategoryImplDeclContext CategoryImplContext { get; set; }

		ObjCTypePorter TypePorter { get; set; }

		public TranslationUnitPorter (CXCursor translationUnit, TranslatorOptions options, IBindingLocator bindingLocator)
		{
			if (translationUnit.kind != CXCursorKind.CXCursor_TranslationUnit)
				throw new ArgumentException ();

			if (options == null)
				throw new ArgumentNullException ();

			if (bindingLocator == null)
				throw new ArgumentNullException ();

			this.translationUnit = translationUnit;
			this.options = options;
			this.bindingLocator = bindingLocator;

			cu = SyntaxFactory.CompilationUnit ();

			UsingDirectiveSyntax[] usings = CreateUsings ();
			foreach (var u in usings)
				cu = cu.AddUsings (u);

			var nsDecl = CreateNamespace (options.Namespace);
			foreach (var c in PortClasses ())
				nsDecl = nsDecl.AddMembers (c);

			cu = cu.AddMembers (nsDecl);
		}

		UsingDirectiveSyntax[] CreateUsings ()
		{
			// TODO: generate usings acording to included files
			return new UsingDirectiveSyntax[] {
				CreateUsing ("System"),
				CreateUsing ("UIKit"),
				CreateUsing ("Foundation")
			};
		}

		UsingDirectiveSyntax CreateUsing (string name)
		{
			return SyntaxFactory.UsingDirective (SyntaxFactory.IdentifierName (name));
		}

		NamespaceDeclarationSyntax CreateNamespace (string ns)
		{
			return SyntaxFactory.NamespaceDeclaration (SyntaxFactory.IdentifierName (ns));
		}

		List<ClassDeclarationSyntax> PortClasses ()
		{
			var classes = new List<ClassDeclarationSyntax> ();
			var children = translationUnit.GetChildren ().Where (c => !IsFromHeader (c));

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
			TypePorter = new ObjCTypePorter (ImplContext.ClassName);

			ClassDeclarationSyntax classDecl = SF.ClassDeclaration (ImplContext.ClassName);
			classDecl = classDecl.AddModifiers (SF.Token (SyntaxKind.PublicKeyword));
			classDecl = classDecl.AddBaseListTypes (SF.SimpleBaseType (SF.ParseTypeName(ImplContext.SuperClassName)));

			var propDecls = ImplContext.DeclCursor.GetChildren ().Where (c => c.kind == CXCursorKind.CXCursor_ObjCPropertyDecl);
			foreach (var pDecl in propDecls)
				classDecl = classDecl.AddMembers (CreatePropertyFromPropertyDecl (pDecl));

			var unrecognized = new List<CXCursor> ();
			List<CXCursor> children = cursor.GetChildren ();
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
			TypePorter = new ObjCTypePorter (CategoryImplContext.ExtendedClassName);

			string className = string.Format ("{0}{1}Extensions", CategoryImplContext.ExtendedClassName, CategoryImplContext.CategoryName);
			ClassDeclarationSyntax classDecl = SF.ClassDeclaration (className);
			classDecl = classDecl.AddModifiers (SF.Token (SyntaxKind.PublicKeyword), SF.Token (SyntaxKind.StaticKeyword));

			var unrecognized = new List<CXCursor> ();
			List<CXCursor> children = cursor.GetChildren ();
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

			ParameterSyntax[] mParams = FetchParamInfos (cursor);

			ParameterSyntax thisParam = SF.Parameter (SF.Identifier ("self"))
				.WithType (SF.ParseTypeName (CategoryImplContext.ExtendedClassName));
			mParams = (new ParameterSyntax[] { thisParam }).Concat (mParams).ToArray ();

			TypeSyntax retType = TypePorter.PortType (objcMethod.ReturnType);
			string methodName = MethodHelper.ConvertToMehtodName (objcMethod.Selector);

			var mb = new MethodBuilder ();
			MethodDeclarationSyntax mDecl = mb.BuildExtensionMethod (retType, methodName, mParams);

			List<CXCursor> children = cursor.GetChildren ();
			var compoundStmt = children.First (c => c.kind == CXCursorKind.CXCursor_CompoundStmt);
			return AddBody (compoundStmt, mDecl);
		}

		PropertyDeclarationSyntax CreatePropertyFromPropertyDecl (CXCursor propDecl)
		{
			CXType propType = clang.getCursorType (propDecl);
			TypeSyntax propTypeSyntax = TypePorter.PortType (propType);

			PropertyDeclarationSyntax propSyntax = SF.PropertyDeclaration (propTypeSyntax, propDecl.ToString())
				.AddModifiers (SF.Token (SyntaxKind.PublicKeyword));

			propSyntax = propSyntax.AddAccessorListAccessors (
				SF.AccessorDeclaration (SyntaxKind.GetAccessorDeclaration)
				.WithSemicolonToken (SF.Token (SyntaxKind.SemicolonToken)),

				SF.AccessorDeclaration (SyntaxKind.SetAccessorDeclaration)
				.WithSemicolonToken (SF.Token (SyntaxKind.SemicolonToken))
			);

			return propSyntax;
		}

		BaseMethodDeclarationSyntax PortMethod (CXCursor cursor)
		{
			var objcMethod = new ObjCMethod (cursor);
			ParameterSyntax[] mParams = FetchParamInfos (cursor);

			MethodDefinition mDef;
			MethodDeclarationSyntax mDecl = null;
			ConstructorDeclarationSyntax ctorDecl = null;

			var mb = new MethodBuilder ();
			var className = ImplContext.GetFirstParentFromSystemFramework ().ToString ();
			var isBound = bindingLocator.TryFindMethod (className, objcMethod.Selector, out mDef);
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

			List<CXCursor> children = cursor.GetChildren ();
			var compoundStmt = children.First (c => c.kind == CXCursorKind.CXCursor_CompoundStmt);

			if (ctorDecl != null)
				return AddBody (compoundStmt, ctorDecl);
			else
				return AddBody (compoundStmt, mDecl);
		}

		MethodDeclarationSyntax BuildDefaultDeclaration (ObjCMethod objcMethod, ParameterSyntax[] mParams)
		{
			TypeSyntax retType = TypePorter.PortType (objcMethod.ReturnType);
			string methodName = MethodHelper.ConvertToMehtodName (objcMethod.Selector);

			var mb = new MethodBuilder ();
			MethodDeclarationSyntax mDecl = mb.BuildDeclaration (retType, methodName, mParams);

			if (objcMethod.IsStatic)
				mDecl = mDecl.WithStaticKeyword ();

			return mDecl;
		}

		ParameterSyntax[] FetchParamInfos (CXCursor methodCursor)
		{
			return methodCursor.GetChildren ()
				.Where (c => c.kind == CXCursorKind.CXCursor_ParmDecl)
				.Select (TypePorter.PortParameter)
				.ToArray ();
		}

		MethodDeclarationSyntax AddBody (CXCursor compountStmt, MethodDeclarationSyntax mDecl)
		{
			return mDecl.AddBodyStatements (NotImplementedStatement (compountStmt));
		}

		ConstructorDeclarationSyntax AddBody (CXCursor compountStmt, ConstructorDeclarationSyntax ctorDecl)
		{
			return ctorDecl.AddBodyStatements (NotImplementedStatement (compountStmt));
		}

		StatementSyntax NotImplementedStatement (CXCursor compountStmt)
		{
			var throwSyntaxToken = SF.Token (SyntaxKind.ThrowKeyword);
			ObjectCreationExpressionSyntax creationExpr = SF.ObjectCreationExpression (SF.ParseTypeName ("NotImplementedException")).WithArgumentList(SF.ArgumentList());
			ThrowStatementSyntax notImplementedStat = SF.ThrowStatement (throwSyntaxToken, creationExpr, SF.Token (SyntaxKind.SemicolonToken));
			if (options.SkipMethodBody)
				return notImplementedStat;

			SyntaxTrivia[] syntaxTrivia = FetchTrivias (compountStmt);
			notImplementedStat = notImplementedStat.WithLeadingTrivia (syntaxTrivia);

			return notImplementedStat;
		}

		SyntaxTrivia[] FetchTrivias (CXCursor compountStmt)
		{
			string methodBody = MethodHelper.GetTextFromCompoundStmt (compountStmt);

			string[] lines = methodBody.Split (new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			SyntaxTrivia[] trivias = new SyntaxTrivia[2 * lines.Length];
			for (int i = 0; i < lines.Length; i ++) {
				trivias [2 * i] = SF.SyntaxTrivia (SyntaxKind.SingleLineCommentTrivia, string.Format ("//{0}", lines[i]));
				trivias [2 * i + 1] = SF.CarriageReturnLineFeed;
			}

			return trivias;
		}

		static bool IsFromHeader (CXCursor cursor)
		{
			return !(cursor.IsFromMainFile () || cursor.kind == CXCursorKind.CXCursor_TranslationUnit);
		}

		public string Generate()
		{
			var ws = new AdhocWorkspace ();
			var formattedNode = Formatter.Format (cu, ws);

			StringBuilder sb = new StringBuilder();
			using (StringWriter writer = new StringWriter (sb))
				formattedNode.WriteTo (writer);

			string result = sb.ToString ();
			return result;
		}
	}
}