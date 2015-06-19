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
			IEnumerable<MethodDefinition> methods = types.SelectMany (t => t.Methods).Where (m => m.IsExported (selector));
			mDef = methods.FirstOrDefault ();

			return mDef != null;
		}

		bool HasRegisterAttribute(TypeDefinition typeDef, string className)
		{
			if (!typeDef.HasCustomAttributes)
				return false;

			return typeDef.CustomAttributes.Where (ca => ca.IsRegister (className)).Any ();
		}
	}
}