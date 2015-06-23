using System;

using Mono.Cecil;
using System.Linq;

namespace Translator.Core
{
	public static class XamarinAttributesExtensions
	{
		public static bool IsExport(this CustomAttribute attribute, string selector)
		{
			if (attribute.AttributeType.FullName != "Foundation.ExportAttribute")
				return false;

			var selectorValue = (string)attribute.ConstructorArguments [0].Value;
			return selector == selectorValue;
		}

		public static bool IsRegistered (this TypeDefinition typeDef, string className)
		{
			if (!typeDef.HasCustomAttributes)
				return false;

			var register = typeDef.CustomAttributes
				.Where (ca => ca.AttributeType.FullName == "Foundation.RegisterAttribute")
				.FirstOrDefault ();

			if (register == null)
				return false;

			if (register.HasConstructorArguments)
				return (string)register.ConstructorArguments [0].Value == className;

			return typeDef.Name == className;
		}

		public static bool IsExported (this MethodDefinition mDef, string selector)
		{
			if (!mDef.HasCustomAttributes)
				return false;

			return mDef.CustomAttributes.Where (ca => ca.IsExport (selector)).Any ();
		}
	}
}