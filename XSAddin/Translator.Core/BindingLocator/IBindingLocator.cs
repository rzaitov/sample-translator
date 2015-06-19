using System;

using Mono.Cecil;

namespace Translator.Core
{
	public interface IBindingLocator
	{
		bool TryFindMethod (string className, string selector, out MethodDefinition mDef);
	}
}

