using System;
using System.Collections.Generic;

using Mono.Cecil;
using System.Linq;

namespace Translator.Core
{
	public class BindingLocator : IBindingLocator
	{
		readonly ModuleDefinition[] modules;

		public BindingLocator (IEnumerable<string> paths)
		{
			modules = paths.Select (p => ModuleDefinition.ReadModule (p)).ToArray ();
		}

		public bool TryFindMethod (string className, string selector, out MethodDefinition mDef)
		{
			IEnumerable<TypeDefinition> types = modules.SelectMany (m => m.Types.Where (t => t.IsRegistered (className)));
			foreach (var t in types) {
				if (TryFindMethod (t, selector, out mDef))
					return true;
			}

			mDef = null;
			return false;
		}

		public bool TryFindMethod (TypeDefinition type, string selector, out MethodDefinition mDef)
		{
			while (type != null) {
				if (TryFindMethodInType (type, selector, out mDef))
					return true;

				type = type.BaseType != null ? type.BaseType.Resolve () : null;
			}

			mDef = null;
			return false;
		}

		public bool TryFindMethodInType (TypeDefinition type, string selector, out MethodDefinition mDef)
		{
			IEnumerable<MethodDefinition> methods = type.Methods.Where (m => m.IsExported (selector));
			mDef = methods.FirstOrDefault ();

			return mDef != null;
		}
	}
}