using System;
using ClangSharp;

namespace Translator.Core
{
	public class ObjCPropertyImplInfo
	{
		public ObjCPropertyDecl Declaration { get; set; }
		public CXCursor? Getter { get; private set; }
		public CXCursor? Setter { get; private set; }
		public CXCursor? ObjCSynthesizeDecl { get; private set; }

		ObjCPropertyImplInfo ()
		{
		}

		public static ObjCPropertyImplInfo FindImplementation (ObjCPropertyDecl propDecl, CXCursor interfaceImpl)
		{
			if (interfaceImpl.kind != CXCursorKind.CXCursor_ObjCImplementationDecl)
				throw new ArgumentException ();

			var impl = new ObjCPropertyImplInfo {
				Declaration = propDecl
			};
			var children = interfaceImpl.GetChildren ();

			foreach (var child in children) {
				if (child.kind == CXCursorKind.CXCursor_ObjCSynthesizeDecl && child.ToString () == propDecl.Name) {
					impl.ObjCSynthesizeDecl = child;
					continue;
				}

				if (child.kind == CXCursorKind.CXCursor_ObjCInstanceMethodDecl) {
					var selector = child.ToString ();
					if (propDecl.GetterName == selector)
						impl.Getter = child;
					else if (propDecl.SetterName == selector && propDecl.IsReadWrite)
						impl.Setter = child;
				}
			}

			return impl;
		}
	}
}