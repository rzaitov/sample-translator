using System;
using ClangSharp;

namespace Translator.Core
{
	public class ObjCProperty
	{
		readonly CXCursor cursor;

		public string Name {
			get {
				return cursor.ToString ();
			}
		}

		public bool IsReadonly {
			get {
				var attrs = (CXObjCPropertyAttrKind)clang.Cursor_getObjCPropertyAttributes (cursor, 0);
				return attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_readonly);
			}
		}

		public bool IsReadWrite {
			get {
				var attrs = (CXObjCPropertyAttrKind)clang.Cursor_getObjCPropertyAttributes (cursor, 0);

				bool hasReadWriteAttr = attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_readwrite);
				bool hasReadonlyAttr = attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_readonly);

				return hasReadWriteAttr || !hasReadonlyAttr;
			}
		}

		string getterName;
		public string GetterName {
			get {
				if (getterName == null) {
					var attrs = (CXObjCPropertyAttrKind)clang.Cursor_getObjCPropertyAttributes (cursor, 0);
					getterName = attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_getter)
						? FindCustomGetterName ()
						: BuildDefaultGetterName ();
				}
				return getterName;
			}
		}

		string setterName;
		public string SetterName {
			get {
				if(IsReadonly)
					return null;

				if(setterName == null) {
					var attrs = (CXObjCPropertyAttrKind)clang.Cursor_getObjCPropertyAttributes (cursor, 0);
					setterName = attrs.HasFlag (CXObjCPropertyAttrKind.CXObjCPropertyAttr_setter)
						? FindCustomSetterName ()
						: BuildDefaultSetterName ();
				}
				return setterName;
			}
		}

		public ObjCProperty (CXCursor cursor)
		{
			if (cursor.kind != CXCursorKind.CXCursor_ObjCPropertyDecl)
				throw new ArgumentException ();

			this.cursor = cursor;
		}

		string FindCustomGetterName ()
		{
			using (TokenGroup tg = cursor.Tokenize ()) {
				int index = IdentifierIndex (tg, "getter");
				return FirstIdentifier (tg, index+1);
			}
		}

		string FindCustomSetterName ()
		{
			using (TokenGroup tg = cursor.Tokenize ()) {
				int index = IdentifierIndex (tg, "setter");
				return FirstIdentifier (tg, index+1);
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
			char[] arr = new char[Name.Length + 3];
			arr[0] = 's';
			arr[1] = 'e';
			arr[2] = 't';

			arr[3] = char.ToUpper(Name[0]);
			for (int i = 1; i < Name.Length; i++)
				arr[i + 3] = Name [i];

			return new String(arr);
		}
	}
}