using System;

using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XISignatureDumper
{
	public class SignatureExtruder
	{
		readonly ModuleDefinition moduleDef;

		public SignatureExtruder (string modulePath)
		{
			moduleDef = ModuleDefinition.ReadModule (modulePath);
		}

		public Dictionary<string, string> CollectSignatures ()
		{
			var map = new Dictionary<string, string> ();

			IEnumerable<MethodDefinition> methods = moduleDef.Types.SelectMany (t => t.Methods);
			foreach (var mDef in methods) {
				if (!mDef.HasCustomAttributes)
					continue;

				if (mDef.IsConstructor)
					continue;

				var exports = mDef.CustomAttributes.Where (ca => ca.AttributeType.FullName == "Foundation.ExportAttribute");
				foreach (var eAttr in exports) {
					var selector = (string)eAttr.ConstructorArguments [0].Value;
					map [selector] = GetSignature (mDef);
				}
			}

			return map;
		}

		string GetSignature (MethodDefinition mDef)
		{
			var sb = new StringBuilder ();
			if (mDef.IsGetter || mDef.IsSetter)
				sb.Append ("/* property */ ");

			if (mDef.IsPublic)
				sb.Append ("public ");
			else if (mDef.IsFamily)
				sb.Append ("protected ");

			if (mDef.IsVirtual)
				sb.Append ("override ");

			sb.Append (PrettifyType(mDef.ReturnType)).Append (" ");
			sb.Append (mDef.Name);

			return sb.ToString ();
		}

		string PrettifyType (TypeReference typeRef)
		{
			if (typeRef.IsArray) {
				var tspec = (TypeSpecification)typeRef;
				return string.Format ("{0}[]", PrettifyAtomicType (tspec.ElementType));
			}

			return PrettifyAtomicType (typeRef);
		}

		string PrettifyAtomicType (TypeReference typeRef)
		{
			switch (typeRef.FullName) {
				case "System.Void":
					return "void";

				case "System.String":
					return "string";

				case "System.Boolean":
					return "bool";

				case "System.Int32":
					return "int";

				default:
					return typeRef.Name;
			}
		}

	}
}

