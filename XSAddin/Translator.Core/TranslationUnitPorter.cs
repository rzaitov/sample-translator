using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;

using ClangSharp;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.CSharp.Formatting;

namespace Translator.Core
{
	public class TranslationUnitPorter
	{
		readonly CXCursor translationUnit;
		CompilationUnitSyntax cu;

		public TranslationUnitPorter (CXCursor translationUnit, string ns)
		{
			if (translationUnit.kind != CXCursorKind.CXCursor_TranslationUnit)
				throw new ArgumentException ();

			if (string.IsNullOrWhiteSpace (ns))
				throw new ArgumentException ();

			this.translationUnit = translationUnit;
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
				if (c.kind == CXCursorKind.CXCursor_ObjCImplementationDecl) {
					classes.Add (PortClass (c));
				} else {
					unrecognized.Add (c);
				}
			}

			// TODO: warn about unrecognized cursors

			return classes;
		}

		ClassDeclarationSyntax PortClass (CXCursor cursor)
		{
			CXCursor super = cursor.GetSuperClass ();

			ClassDeclarationSyntax classDecl = SyntaxFactory.ClassDeclaration (cursor.ToString ());
			classDecl = classDecl.AddModifiers (SyntaxFactory.Token (SyntaxKind.PublicKeyword));
			classDecl = classDecl.AddBaseListTypes (SyntaxFactory.SimpleBaseType (SyntaxFactory.ParseTypeName(super.ToString())));

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

		MethodDeclarationSyntax PortMethod (CXCursor cursor)
		{
			if (cursor.kind != CXCursorKind.CXCursor_ObjCInstanceMethodDecl &&
			    cursor.kind != CXCursorKind.CXCursor_ObjCClassMethodDecl)
				throw new ArgumentException ();

			var children = cursor.GetChildren ();
			CXCursor returnType = children.First (c => c.kind == CXCursorKind.CXCursor_TypeRef);

			string selector = cursor.ToString ();
			string methodName = MethodHelper.ConvertToMehtodName (selector);

			MethodDeclarationSyntax mDecl = SyntaxFactory.MethodDeclaration (SyntaxFactory.ParseTypeName (returnType.ToString ()), methodName);
			mDecl = mDecl.AddModifiers (SyntaxFactory.Token (SyntaxKind.PublicKeyword));
			if(cursor.kind == CXCursorKind.CXCursor_ObjCClassMethodDecl)
				mDecl = mDecl.AddModifiers (SyntaxFactory.Token (SyntaxKind.StaticKeyword));

			// TODO: add parameters

			foreach (var c in children) {
				if (c.kind != CXCursorKind.CXCursor_CompoundStmt)
					continue;

				string methodBody = c.GetBodyText ();
				Console.WriteLine (methodBody);
				IEnumerable<string> lines = MethodHelper.Comment (methodBody);
				var trivias = lines.Select (l => SyntaxFactory.SyntaxTrivia (SyntaxKind.SingleLineCommentTrivia, l));
//				SyntaxTrivia comment = SyntaxFactory.Comment (methodBody);
//				mDecl = mDecl.AddBodyStatements ( comment);
				mDecl = mDecl.AddBodyStatements (new StatementSyntax[0]);
				SyntaxToken cl = mDecl.Body.CloseBraceToken.WithLeadingTrivia (trivias);
				mDecl = mDecl.ReplaceToken(mDecl.Body.CloseBraceToken, cl);

			}

			return mDecl;
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