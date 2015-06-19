using System;

using Mono.Cecil;

namespace Translator.Core
{
	public class EmptyBindingLocator : IBindingLocator
	{
		public bool TryFindMethod (string className, string selector, out MethodDefinition mDef)
		{
			mDef = null;
			return false;
		}
	}
}