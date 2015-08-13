using System;
using ClangSharp;
using System.Text;

namespace Translator.Core
{
	public class ObjCPropertyDecl
	{
		readonly CXCursor propDeclCursor;

		public CXCursor DeclarationCursor {
			get {
				return propDeclCursor;
			}
		}

		public string Name {
			get {
				return propDeclCursor.ToString ();
			}
		}

		public bool IsReadonly {
			get {
				var attrs = (CXObjCPropertyAttrKind)clang.Cursor_getObjCPropertyAttributes (propDeclCursor, 0);
				return attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_readonly);
			}
		}

		public bool IsReadWrite {
			get {
				var attrs = (CXObjCPropertyAttrKind)clang.Cursor_getObjCPropertyAttributes (propDeclCursor, 0);

				bool hasReadWriteAttr = attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_readwrite);
				bool hasReadonlyAttr = attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_readonly);

				return hasReadWriteAttr || !hasReadonlyAttr;
			}
		}

		public CXCursor Getter { get; private set; }

		string getterName;
		public string GetterName {
			get {
				if (getterName == null) {
					var attrs = (CXObjCPropertyAttrKind)clang.Cursor_getObjCPropertyAttributes (propDeclCursor, 0);
					getterName = attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_getter)
						? FindCustomGetterName ()
						: BuildDefaultGetterName ();
				}
				return getterName;
			}
		}

		public CXCursor? Setter { get; set; }

		string setterName;
		public string SetterName {
			get {
				if(IsReadonly)
					return null;

				if(setterName == null) {
					var attrs = (CXObjCPropertyAttrKind)clang.Cursor_getObjCPropertyAttributes (propDeclCursor, 0);
					setterName = attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_setter)
						? FindCustomSetterName ()
						: BuildDefaultSetterName ();
				}
				return setterName;
			}
		}

		public ObjCPropertyDecl (CXCursor cursor)
		{
			if (cursor.kind != CXCursorKind.CXCursor_ObjCPropertyDecl)
				throw new ArgumentException ();

			propDeclCursor = cursor;
			FindAccessors ();
		}

		void FindAccessors ()
		{
			var parent = clang.getCursorSemanticParent (propDeclCursor);
			foreach (var c in  parent.GetChildren()) {
				if (IsGetter (c))
					Getter = c;
				else if (IsSetter (c))
					Setter = c;
			}
		}

		bool IsGetter(CXCursor cursor)
		{
			if (cursor.kind != CXCursorKind.CXCursor_ObjCInstanceMethodDecl)
				return false;

			return cursor.ToString () == GetterName;
		}

		bool IsSetter (CXCursor cursor)
		{
			if (cursor.kind != CXCursorKind.CXCursor_ObjCInstanceMethodDecl)
				return false;

			return cursor.ToString () == SetterName;
		}

		string FindCustomGetterName ()
		{
			using (TokenGroup tg = propDeclCursor.Tokenize ()) {
				int index = IdentifierIndex (tg, "getter");
				return FirstIdentifier (tg, index + 1);
			}
		}

		string FindCustomSetterName ()
		{
			using (TokenGroup tg = propDeclCursor.Tokenize ()) {
				int index = IdentifierIndex (tg, "setter");
				return string.Format ("{0}:", FirstIdentifier (tg, index + 1));
			}
		}

		int IdentifierIndex(TokenGroup tg, string name, int start = 0)
		{
			for (int i = start; i < tg.Tokens.Length; i++) {
				CXToken token = tg.Tokens [i];
				if (token.Kind () == CXTokenKind.CXToken_Identifier && token.Spellings (tg.TranslationUnit) == name)
					return i;
			}

			throw new InvalidProgramException();
		}

		string FirstIdentifier (TokenGroup tg, int start)
		{
			for (int i = start; i < tg.Tokens.Length; i++) {
				CXToken token = tg.Tokens [i];
				if (token.Kind () == CXTokenKind.CXToken_Identifier)
					return token.Spellings (tg.TranslationUnit);
			}

			throw new InvalidProgramException();
		}

		string BuildDefaultGetterName ()
		{
			return Name;
		}

		string BuildDefaultSetterName ()
		{
			var sb = new StringBuilder ("set", Name.Length + 4); // "setPropName:"
			sb.Append (char.ToUpper (Name [0]));

			for (int i = 1; i < Name.Length; i++)
				sb.Append (Name [i]);

			sb.Append (':');

			return sb.ToString ();
		}
	}
}