using System;

namespace Translator.Core
{
	public class ObjCTypeNamePrettifier
	{
		readonly string idTypeName;

		public ObjCTypeNamePrettifier (string idTypeName)
		{
			this.idTypeName = idTypeName;
		}

		public string Prettify (string rawTypeName)
		{
			switch (rawTypeName) {
			case "id":
				return idTypeName;

			case "CGFloat":
				return "nfloat";

			default:
				return rawTypeName;
			}
		}
	}
}