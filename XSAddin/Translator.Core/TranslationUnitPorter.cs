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
		readonly BindingLocator bindingLocator;
		readonly CXCursor translationUnit;
		CompilationUnitSyntax cu;

		CXCursor currentClass;

		public TranslationUnitPorter (CXCursor translationUnit, string ns, BindingLocator bindingLocator)
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
			if (cursor.kind != CXCursorKind.CXCursor_ObjCImplementationDecl)
				throw new ArgumentException ();
			currentClass = cursor;
			CXCursor super = cursor.GetSuperClass (); // CXCursor_ObjCSuperClassRef

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
			var objcMethod = new ObjCMethod (cursor);

			string baseClassName = currentClass.GetSuperClass ().ToString ();

			IEnumerable<CXCursor> children = cursor.GetChildren ();
			IEnumerable<Tuple<string, string>> mParams = children
				.Where (c => c.kind == CXCursorKind.CXCursor_ParmDecl)
				.Select (CreateParamInfo);

			MethodDefinition mDef;
			MethodDeclarationSyntax mDecl;

			if (bindingLocator.TryFindMethod (baseClassName, objcMethod.Selector, out mDef)) {
				var mb = new MethodBuilder ();
				mDecl = mb.BuildDeclaration (mDef, mParams);
			} else {
				mDecl = BuildDefaultDeclaration (objcMethod, mParams);
			}

			var compoundStmt = children.First (c => c.kind == CXCursorKind.CXCursor_CompoundStmt);
			mDecl = AddMethodBody (compoundStmt, mDecl);

			return mDecl;
		}

		MethodDeclarationSyntax BuildDefaultDeclaration (ObjCMethod objcMethod, IEnumerable<Tuple<string, string>> mParams)
		{
			string retTypeName = PrettifyTypeName (objcMethod.ReturnType.ToString ());
			string methodName = MethodHelper.ConvertToMehtodName (objcMethod.Selector);

			var mb = new MethodBuilder ();
			MethodDeclarationSyntax mDecl = mb.BuildDefaultDeclaration (retTypeName, methodName, mParams);

			if(objcMethod.IsStatic)
				mDecl = mDecl.AddModifiers (SF.Token (SyntaxKind.StaticKeyword));

			return mDecl;
		}

		Tuple<string, string> CreateParamInfo (CXCursor parmDecl)
		{
			string paramName = parmDecl.ToString ();
			CXCursor typeRef = parmDecl.GetChildren ().First (); // CXCursor_TypeRef
			string typeName = typeRef.ToString ();

			return new Tuple<string, string> (typeName, paramName);
		}

		ParameterSyntax PortParameter (CXCursor parmDecl)
		{
			string paramName = parmDecl.ToString ();
			CXCursor typeRef = parmDecl.GetChildren ().First (); // CXCursor_TypeRef
			string typeName = typeRef.ToString ();

			return SyntaxFactory.Parameter(SyntaxFactory.Identifier(paramName))
				.WithType(SyntaxFactory.ParseTypeName(PrettifyTypeName(typeName)));
		}

		MethodDeclarationSyntax AddMethodBody (CXCursor compountStmt, MethodDeclarationSyntax mDecl)
		{
			string methodBody = MethodHelper.GetTextFromCompoundStmt (compountStmt);
			IEnumerable<string> lines = MethodHelper.Comment (methodBody);
			var trivias = lines.Select (l => SyntaxFactory.SyntaxTrivia (SyntaxKind.SingleLineCommentTrivia, l));

			mDecl = mDecl.AddBodyStatements (new StatementSyntax[0]);
			SyntaxToken cl = mDecl.Body.CloseBraceToken.WithLeadingTrivia (trivias);
			return mDecl.ReplaceToken(mDecl.Body.CloseBraceToken, cl);
		}

		string PrettifyTypeName (string rawTypeName)
		{
			if (rawTypeName == "id")
				return currentClass.ToString ();

			return rawTypeName;
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