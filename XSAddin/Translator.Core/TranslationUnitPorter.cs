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

namespace Translator.Core
{
	public class TranslationUnitPorter
	{
		readonly CXCursor translationUnit;
		CompilationUnitSyntax cu;

		NamespaceDeclarationSyntax nsDecl;

		public TranslationUnitPorter (CXCursor translationUnit, string ns)
		{
			if (translationUnit.kind != CXCursorKind.CXCursor_TranslationUnit)
				throw new ArgumentException ();

			if (string.IsNullOrWhiteSpace (ns))
				throw new ArgumentException ();

			this.translationUnit = translationUnit;
			cu = SyntaxFactory.CompilationUnit ();

			AddUsings ();
			nsDecl = AddNamespace (ns);
			PortClasses (ref nsDecl);
//			nsDecl.SyntaxTree.ToString
		}

		void AddUsings ()
		{
			// TODO: generate usings acording to included files
			cu = cu.AddUsings (CreateUsing ("System"), CreateUsing ("UIKit"), CreateUsing ("Foundation"));
		}

		UsingDirectiveSyntax CreateUsing (string name)
		{
			return SyntaxFactory.UsingDirective (SyntaxFactory.IdentifierName (name));
		}

		NamespaceDeclarationSyntax AddNamespace (string ns)
		{
			NamespaceDeclarationSyntax nsDecl = SyntaxFactory.NamespaceDeclaration (SyntaxFactory.IdentifierName (ns));
			cu = cu.AddMembers (nsDecl);

			return nsDecl;
		}

		void PortClasses (ref NamespaceDeclarationSyntax nsDecl)
		{
			IEnumerable<CXCursor> children = translationUnit.GetChildren ().Where (c => !IsFromHeader(c));

			var unrecognized = new List<CXCursor> ();
			foreach (var c in children) {
				if (c.kind == CXCursorKind.CXCursor_ObjCImplementationDecl)
					PortClass (c, ref nsDecl);
				else
					unrecognized.Add (c);
			}

			// TODO: warn about unrecognized cursors
		}

		void PortClass (CXCursor cursor, ref NamespaceDeclarationSyntax nsDecl)
		{
			CXCursor super = cursor.GetSuperClass ();

			ClassDeclarationSyntax classDecl = SyntaxFactory.ClassDeclaration (cursor.ToString ());
			classDecl = classDecl.AddModifiers (SyntaxFactory.Token (SyntaxKind.PublicKeyword));
			classDecl = classDecl.AddBaseListTypes (SyntaxFactory.SimpleBaseType (SyntaxFactory.ParseTypeName(super.ToString())));

			var unrecognized = new List<CXCursor> ();
			IEnumerable<CXCursor> children = cursor.GetChildren ();
			foreach (var c in children) {
				if (c.kind == CXCursorKind.CXCursor_ObjCInstanceMethodDecl || c.kind == CXCursorKind.CXCursor_ObjCClassMethodDecl)
					PortMethod (c, ref classDecl);
				else
					unrecognized.Add (c);
			}

			// TODO: warn about unrecognized cursors

			nsDecl = nsDecl.AddMembers (classDecl);
		}

		void PortMethod (CXCursor cursor, ref ClassDeclarationSyntax classDecl)
		{
			if (cursor.kind != CXCursorKind.CXCursor_ObjCInstanceMethodDecl &&
			    cursor.kind != CXCursorKind.CXCursor_ObjCClassMethodDecl)
				throw new ArgumentException ();

			var children = cursor.GetChildren ();
			var returnType = children.First ();

			string selector = cursor.ToString ();
			string[] items = selector.Split (new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
			string methodName = string.Join (string.Empty, items.Select (s => {
				var chars = s.ToCharArray ();
				chars [0] = char.ToUpper (chars [0]);
				return new string (chars);
			}));

			MethodDeclarationSyntax mDecl = SyntaxFactory.MethodDeclaration (SyntaxFactory.ParseTypeName (returnType.ToString ()), methodName);
			mDecl = mDecl.AddModifiers (SyntaxFactory.Token (SyntaxKind.PublicKeyword));
			if(cursor.kind == CXCursorKind.CXCursor_ObjCClassMethodDecl)
				mDecl = mDecl.AddModifiers (SyntaxFactory.Token (SyntaxKind.StaticKeyword));

			// TODO: add parameters

			foreach (var c in children) {
				if (c.kind != CXCursorKind.CXCursor_CompoundStmt)
					continue;

//				string methodBody = c.GetBodyText ();
//				SyntaxTrivia comment = SyntaxFactory.Comment (methodBody);
//				mDecl = mDecl.AddBodyStatements ( comment);
			}

			classDecl = classDecl.AddMembers (mDecl);
		}

		static bool IsFromHeader (CXCursor cursor)
		{
			return !(cursor.IsFromMainFile () || cursor.kind == CXCursorKind.CXCursor_TranslationUnit);
		}

		public string Generate()
		{
			var projectId = ProjectId.CreateNewId();
			var documentId = DocumentId.CreateNewId(projectId);

			var sln = new AdhocWorkspace ().CurrentSolution
				.AddProject (projectId, "translator", "translator", LanguageNames.CSharp)
				.AddDocument (documentId, "somefile.cs", cu.ToString ());

			Document document = sln.GetDocument (documentId);
			return Formatter.FormatAsync (document).Result.ToString ();
//			Formatter.Format(cu, new Wor
//			return cu.SyntaxTree;
//			return nsDecl.SyntaxTree.r
		}
	}
}