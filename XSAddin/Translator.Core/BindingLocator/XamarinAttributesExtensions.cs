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

		public static bool IsRegister(this CustomAttribute attribute, string className)
		{
			if (attribute.AttributeType.FullName != "Foundation.RegisterAttribute")
				return false;

			var selectorValue = (string)attribute.ConstructorArguments [0].Value;
			return className == selectorValue;
		}

		public static bool IsRegistered (this TypeDefinition typeDef, string className)
		{
			if (!typeDef.HasCustomAttributes)
				return false;

			return typeDef.CustomAttributes.Where (ca => ca.IsRegister (className)).Any ();
		}

		public static bool IsExported (this MethodDefinition mDef, string selector)
		{
			if (!mDef.HasCustomAttributes)
				return false;

			return mDef.CustomAttributes.Where (ca => ca.IsExport (selector)).Any ();
		}
	}
}